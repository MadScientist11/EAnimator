namespace EAnimator
{
    public abstract class EventParameter<T>
    {
        public T Value { get; set; }
    }

    public class IntParameter : EventParameter<int>
    {
    }

    public class StringParameter : EventParameter<string>
    {
    }

    public class ObjectParameter : EventParameter<UnityEngine.Object>
    {
    }
}

