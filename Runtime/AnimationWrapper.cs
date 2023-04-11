using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EAnimator
{
  

    public class AnimationWrapper
    {
        private readonly Animator _animator;
        private readonly int _animationHash;
        private readonly string _animationName;
        private readonly AnimationProperties _animationProperties;
        private readonly List<EventArgs> _addedEventArgs;

        private AnimationClip _animationClip;


        public AnimationWrapper(Animator animator, int animationHash, string animationName,
            AnimationProperties animationProperties)
        {
            _animator = animator;
            _animationHash = animationHash;
            _animationName = animationName;
            _animationProperties = animationProperties;
            _addedEventArgs = new List<EventArgs>();
            _animationClip =
                _animator.runtimeAnimatorController.animationClips.FirstOrDefault(ac => ac.name == _animationName);
        }

        public void Enter()
        {
            RestoreEvents();
            AddAnimationEventCallbackComponent();
            _animator.SetFloat($"{_animationName}Speed", _animationProperties.Speed);
            _animator.Play(_animationHash, _animationProperties.Layer, _animationProperties.NormalizedTime);
        }

        public void Exit()
        {
            DeleteAddedEvents();
        }

        public AnimationWrapper AddEvent<TArgs>(TArgs eventArgs) where TArgs : EventArgs
        {
            _addedEventArgs.Add(eventArgs);
            return this;
        }

        private AnimationWrapper AddEventInternal<TArgs>(TArgs eventArgs) where TArgs : EventArgs
        {
            if (_animationClip == null)
            {
                Debug.LogError($"Failed to create an Event, AnimationClip with the name {_animationName} doesn't exist on current Animator");
                return this;
            }

            if (EventWithTheTypeWasAlreadyAdded(eventArgs))
            {
                Debug.LogError($"Failed to create an Event with the type {eventArgs.GetType()}. " +
                               "Event with the same signature already exists, try using another EventArgs type");
                return this;
            }

            AnimationEvent animationEvent = CreateAnimationEvent(eventArgs);
            
            _animationClip.AddEvent(animationEvent);
            _addedEventArgs.Add(eventArgs);
            return this;
        }

        private void AddAnimationEventCallbackComponent()
        {
            if(_addedEventArgs.Count == 0) return;
            
            if (_animator.gameObject.TryGetComponent(out AnimationEventComponent eventComponent))
            {
                UnityEngine.Object.Destroy(eventComponent);
            }

            AnimationEventComponent animEventComponent = _animator.gameObject.AddComponent<AnimationEventComponent>();
            
            foreach (var eventArgs in _addedEventArgs)
            {
                animEventComponent.Initialize(eventArgs);
            }
        }

        private bool EventWithTheTypeWasAlreadyAdded<TArgs>(TArgs eventArgs) where TArgs : EventArgs
        {
            foreach (EventArgs addedEventArgs in _addedEventArgs)
            {
                if (addedEventArgs.GetType() == eventArgs.GetType())
                {
        
                    return true;
                }
            }

            return false;
        }

     
        private void RestoreEvents()
        {
            List<EventArgs> eventArgsList = _addedEventArgs.ToList();
            _addedEventArgs.Clear();
            foreach (var eventArgs in eventArgsList)
            {
                if (_animationClip.events.Any(x => x.functionName == eventArgs.CallbackName)) return;

                switch (eventArgs)
                {
                    case ParameterlessEventArgs parameterlessEventArgs:
                        AddEventInternal(parameterlessEventArgs);
                        break;
                    case IntEventArgs intEventArgs:
                        AddEventInternal(intEventArgs);
                        break;
                    case StringEventArgs stringEventArgs:
                        AddEventInternal(stringEventArgs);
                        break;
                    case ObjectEventArgs objectEventArgs:
                        AddEventInternal(objectEventArgs);
                        break;
                }
            }
        }

        private void DeleteAddedEvents()
        {
            if (_addedEventArgs.Count == 0 || _animationClip == null) return;

            List<AnimationEvent> eventsList = _animationClip.events.ToList();

            foreach (var animEvent in _addedEventArgs)
            {
                eventsList.RemoveAll(x => x.functionName == animEvent.CallbackName);
            }

            _animationClip.events = eventsList.ToArray();
        }

        private AnimationEvent CreateAnimationEvent(EventArgs eventArgs)
        {
            return eventArgs switch
            {
                ParameterlessEventArgs args1 => new AnimationEvent
                {
                    functionName = args1.CallbackName,
                    time = args1.EventTriggerTime,
                    messageOptions = SendMessageOptions.DontRequireReceiver
                },
                IntEventArgs intArgs => new AnimationEvent
                {
                    functionName = intArgs.CallbackName,
                    time = intArgs.EventTriggerTime,
                    intParameter = intArgs.Value,
                    messageOptions = SendMessageOptions.DontRequireReceiver
                },
                StringEventArgs stringArgs => new AnimationEvent
                {
                    functionName = stringArgs.CallbackName,
                    time = stringArgs.EventTriggerTime,
                    stringParameter = stringArgs.Value,
                    messageOptions = SendMessageOptions.DontRequireReceiver
                },
                ObjectEventArgs objectEventArgs => new AnimationEvent
                {
                    functionName = objectEventArgs.CallbackName,
                    time = objectEventArgs.EventTriggerTime,
                    objectReferenceParameter = objectEventArgs.Value,
                    messageOptions = SendMessageOptions.DontRequireReceiver
                },
                _ => throw new ArgumentOutOfRangeException(nameof(eventArgs)),
            };
        }
    }
}