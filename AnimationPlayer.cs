namespace EAnimator
{
    public class AnimationPlayer
    {
        private AnimationWrapper _currentAnimation;

        public void PlayAnimation(AnimationWrapper animationWrapper)
        {
            _currentAnimation?.Exit();
            _currentAnimation = animationWrapper;
            _currentAnimation.Enter();
        }
    }
}