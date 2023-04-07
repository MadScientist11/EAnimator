using UnityEngine;

namespace EAnimator.Editor
{
    [CreateAssetMenu(fileName = nameof(EAnimatorSettings), menuName = "EAnimator/EAnimatorSettings")]
    public class EAnimatorSettings : ScriptableObject
    {
        public string Path;

        private void OnValidate()
        {
            EditorAnimatorUpdate.UpdateAllAnimators();
        }
    }
}