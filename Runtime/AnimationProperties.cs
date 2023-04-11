namespace EAnimator
{
    public struct AnimationProperties
    {
        public float Speed { get; }
        public int Layer { get; }
        public float NormalizedTime { get; }

        public static AnimationProperties Default()
        {
            return new AnimationProperties(1f, -1, float.NegativeInfinity);
        }

        public AnimationProperties WithSpeed(float speed)
        {
            return new AnimationProperties(speed, Layer, NormalizedTime);
        }

        public AnimationProperties Create(float speed, float normalizedTime)
        {
            return new AnimationProperties(speed, Layer, normalizedTime);
        }

        private AnimationProperties(float speed, int layer, float normalizedTime)
        {
            Speed = speed;
            Layer = layer;
            NormalizedTime = normalizedTime;
        }
    }
}