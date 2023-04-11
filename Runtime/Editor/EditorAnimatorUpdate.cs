using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EAnimator.Editor
{
    [InitializeOnLoad]
    public static class EditorAnimatorUpdate
    {
        private static readonly EAnimatorSettings _settings;

        static EditorAnimatorUpdate()
        {
            string settingsGuid = AssetDatabase.FindAssets("t:EAnimatorSettings").FirstOrDefault();
            string settingsPath = AssetDatabase.GUIDToAssetPath(settingsGuid);

            _settings = AssetDatabase.LoadAssetAtPath<EAnimatorSettings>(settingsPath);

            EditorApplication.update += OnAnimatorWindowSelected;
            UpdateAllAnimators();
        }

        public static void UpdateAllAnimators()
        {
            if (!SettingsInitialized()) return;

            string[] assetGUIDs = AssetDatabase.FindAssets("t:AnimatorController", _settings.Paths);

            foreach (string assetGUID in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                AnimatorController animator =
                    AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
                if (animator != null)
                {
                    UpdateSpeedParametersForStates(animator, animator.layers[0].stateMachine);
                }
            }
        }


        private static void OnAnimatorWindowSelected()
        {
            if (!SettingsInitialized()) return;

            if (Selection.activeObject is AnimatorStateMachine)
            {
                AnimatorController animatorController = GetAnimatorControllerFromWindow(EditorWindow.focusedWindow);

                if (animatorController is null) return;

                AnimatorStateMachine stateMachine = (AnimatorStateMachine)Selection.activeObject;

                UpdateSpeedParametersForStates(animatorController, stateMachine);
            }
        }


        private static void UpdateSpeedParametersForStates(AnimatorController animatorController,
            AnimatorStateMachine stateMachine)
        {
            if (!SettingsInitialized()) return;

            string assetPath = AssetDatabase.GetAssetPath(animatorController);
            string folder = Path.GetDirectoryName(assetPath);


            if (!_settings.Paths.Any(x => ComparePaths(folder, x))) return;

            foreach (ChildAnimatorState state in stateMachine.states)
            {
                string stateName = state.state.name;
                AnimatorState animatorState = state.state;

                if (!animatorController.parameters.Any(x => x.name == $"{stateName}Speed"))
                {
                    animatorController.AddParameter(
                        new AnimatorControllerParameter
                        {
                            name = $"{stateName}Speed",
                            type = AnimatorControllerParameterType.Float,
                            defaultFloat = 1f
                        });
                    animatorState.speedParameterActive = true;
                    animatorState.speedParameter = $"{stateName}Speed";
                }
                else
                {
                    animatorState.speedParameterActive = true;
                    animatorState.speedParameter = $"{stateName}Speed";
                }
            }

            foreach (AnimatorControllerParameter parameter in animatorController.parameters)
            {
                if (parameter.name.EndsWith("Speed"))
                {
                    string stateName = parameter.name.Split("Speed")[0];

                    bool isStateExist = stateMachine.states.Any(x => x.state.name == stateName);
                    if (!isStateExist)
                    {
                        animatorController.RemoveParameter(parameter);
                    }
                }
            }
        }


        private static AnimatorController GetAnimatorControllerFromWindow(EditorWindow window)
        {
            if (window == null) return null;

            Type windowType = window.GetType();
            PropertyInfo controllerProperty = windowType.GetProperty("animatorController",
                BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Instance);

            if (controllerProperty != null)
            {
                return controllerProperty.GetValue(window, null) as AnimatorController;
            }

            return null;
        }

        private static bool SettingsInitialized() =>
            _settings != null && _settings.Paths is { Length: > 0 };

        private static bool ComparePaths(string path1, string path2)
        {
            return string.Equals(path1
                    .Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar),
                path2
                    .Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}