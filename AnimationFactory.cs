namespace EAnimator
{
    public class AnimationFactory
    {
        private readonly UnityEngine.Animator _animator;

        public AnimationFactory(UnityEngine.Animator animator)
        {
            _animator = animator;
        }
        public AnimationWrapper CreateAnimation(int animationHash, string animationName,
            AnimationProperties animationProperties)
        {
            return new AnimationWrapper(_animator, animationHash, animationName, animationProperties);
        }
    }
}