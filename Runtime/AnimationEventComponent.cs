using UnityEngine;

namespace EAnimator
{
    public class AnimationEventComponent : MonoBehaviour
    {
        private ParameterlessEventArgs _parameterlessEventArgs;
        private IntEventArgs _intEventArgs;
        private StringEventArgs _stringEventArgs;
        private ObjectEventArgs _objectEventArgs;

        public void Initialize<T>(T args) where T : EventArgs
        {
            switch (args)
            {
                case ParameterlessEventArgs parameterlessEventArgs:
                    _parameterlessEventArgs = parameterlessEventArgs;
                    break;
                case IntEventArgs intEventArgs:
                    _intEventArgs = intEventArgs;
                    break;
                case StringEventArgs stringEventArgs:
                    _stringEventArgs = stringEventArgs;
                    break;
                case ObjectEventArgs objectEventArgs:
                    _objectEventArgs = objectEventArgs;
                    break;
            }
        }

        public void ParameterlessEventCallback()
        {
            _parameterlessEventArgs.EventAction?.Invoke();
        }

        public void IntEventCallback()
        {
            _intEventArgs.EventAction?.Invoke(_intEventArgs.Value);
        }

        public void StringEventCallback()
        {
            _stringEventArgs.EventAction?.Invoke(_stringEventArgs.Value);
        }   
        
        public void ObjectEventCallback()
        {
            _objectEventArgs.EventAction?.Invoke(_objectEventArgs.Value);
        }

    }
}