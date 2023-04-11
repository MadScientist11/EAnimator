using System;

namespace EAnimator
{
    public abstract class EventArgs
    {
        public abstract string CallbackName { get; }

        public float EventTriggerTime;
    }
    
    public class ParameterlessEventArgs : EventArgs
    {
        public override string CallbackName => nameof(AnimationEventComponent.ParameterlessEventCallback);

        public Action EventAction;
    }

    public class IntEventArgs : EventArgs
    {
        public override string CallbackName => nameof(AnimationEventComponent.IntEventCallback);

        public int Value;
        public Action<int> EventAction;
    }

    public class StringEventArgs : EventArgs
    {
        public override string CallbackName => nameof(AnimationEventComponent.StringEventCallback);

        public string Value;
        public Action<string> EventAction;
    }
    
    public class ObjectEventArgs : EventArgs
    {
        public override string CallbackName => nameof(AnimationEventComponent.ObjectEventCallback);

        public UnityEngine.Object Value;
        public Action<UnityEngine.Object> EventAction;
    }
}