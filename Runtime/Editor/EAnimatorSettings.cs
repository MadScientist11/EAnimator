using UnityEngine;

namespace EAnimator.Editor
{
    [CreateAssetMenu(fileName = nameof(EAnimatorSettings), menuName = "EAnimator/EAnimatorSettings")]
    public class EAnimatorSettings : ScriptableObject
    {
        public string[] Paths;

        private void OnValidate()
        {
            EditorAnimatorUpdate.UpdateAllAnimators();
        }
    }
}