using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EAnimator
{
    public enum ParameterType
    {
        None,
        Int,
        String,
        Object
    }

    public class AnimationWrapper
    {
        private readonly Animator _animator;
        private readonly int _animationHash;
        private readonly string _animationName;
        private readonly AnimationProperties _animationProperties;
        private readonly List<(AnimationEvent animEvent, ParameterType parameterType)> _addedAnimationEvents;

        private AnimationClip _animationClip;

        private Action _onPlay;
        private Action _onTransition;

        public AnimationWrapper(Animator animator, int animationHash, string animationName,
            AnimationProperties animationProperties)
        {
            _animator = animator;
            _animationHash = animationHash;
            _animationName = animationName;
            _animationProperties = InitAnimationProperties(animationProperties);
            _addedAnimationEvents = new List<(AnimationEvent, ParameterType)>();
        }

        private AnimationProperties InitAnimationProperties(AnimationProperties animationProperties)
        {
            animationProperties.Speed ??= _animator.GetFloat($"{_animationName}Speed");
            animationProperties.NormalizedTime ??= float.NegativeInfinity;
            animationProperties.Layer ??= -1;
            return animationProperties;
        }

        public void Enter()
        {
            RestoreEvents();
            _animator.SetFloat($"{_animationName}Speed", _animationProperties.Speed.Value);
            _animator.Play(_animationHash, _animationProperties.Layer.Value, _animationProperties.NormalizedTime.Value);
            _onPlay?.Invoke();
        }

        public void Exit()
        {
            DeleteAddedEvents();
            _onTransition?.Invoke();
        }

        public AnimationWrapper AddEvent<T>(string functionName, float eventTime, EventParameter<T> parameter)
        {
            _animationClip =
                _animator.runtimeAnimatorController.animationClips.FirstOrDefault(ac => ac.name == _animationName);

            if (_animationClip == null)
            {
                Debug.LogError(
                    $"You're trying to add an event, but AnimationClip with the name {_animationName} doesn't exist");
            }

            var animationEvent = CreateAnimationEvent(functionName, eventTime, parameter);
            _animationClip.AddEvent(animationEvent.animEvent);
            _addedAnimationEvents.Add(animationEvent);
            return this;
        }

        public AnimationWrapper AddEvent(string functionName, float eventTime)
        {
            AddEvent<ObjectParameter>(functionName, eventTime, null);
            return this;
        }

        public AnimationWrapper OnPlay(Action after)
        {
            _onPlay = after;
            return this;
        }

        public AnimationWrapper OnTransition(Action after)
        {
            _onTransition = after;
            return this;
        }

        private void RestoreEvents()
        {
            var animationEvents = _addedAnimationEvents.ToList();
            foreach (var animationEvent in animationEvents)
            {
                if (_animationClip.events.Any(x => x.functionName == animationEvent.animEvent.functionName)) return;

                switch (animationEvent.parameterType)
                {
                    case ParameterType.None:
                        AddEvent(animationEvent.animEvent.functionName, animationEvent.animEvent.time);
                        break;
                    case ParameterType.Int:
                        AddEvent(animationEvent.animEvent.functionName, animationEvent.animEvent.time,
                            new IntParameter { Value = animationEvent.animEvent.intParameter });
                        break;
                    case ParameterType.String:
                        AddEvent(animationEvent.animEvent.functionName, animationEvent.animEvent.time,
                            new StringParameter { Value = animationEvent.animEvent.stringParameter });
                        break;
                    case ParameterType.Object:
                        AddEvent(animationEvent.animEvent.functionName, animationEvent.animEvent.time,
                            new ObjectParameter { Value = animationEvent.animEvent.objectReferenceParameter });
                        break;
                }
            }
        }

        private void DeleteAddedEvents()
        {
            if (_addedAnimationEvents.Count == 0 || _animationClip == null) return;

            List<AnimationEvent> eventsList = _animationClip.events.ToList();

            foreach (var animEvent in _addedAnimationEvents)
            {
                eventsList.RemoveAll(x => x.functionName == animEvent.animEvent.functionName);
            }

            _animationClip.events = eventsList.ToArray();
        }

        private (AnimationEvent animEvent, ParameterType parameterType) CreateAnimationEvent<T>(string functionName,
            float eventTime,
            EventParameter<T> parameter)
        {
            return parameter switch
            {
                null => (
                    new AnimationEvent
                    {
                        functionName = functionName, time = eventTime,
                        messageOptions = SendMessageOptions.DontRequireReceiver
                    }, ParameterType.None),
                IntParameter intParameter => (new AnimationEvent
                    {
                        functionName = functionName, time = eventTime, intParameter = intParameter.Value,
                        messageOptions = SendMessageOptions.DontRequireReceiver
                    },
                    ParameterType.Int),
                StringParameter stringParameter => (new AnimationEvent
                    {
                        functionName = functionName, time = eventTime, stringParameter = stringParameter.Value,
                        messageOptions = SendMessageOptions.DontRequireReceiver
                    },
                    ParameterType.String),
                ObjectParameter objectParameter => (new AnimationEvent
                    {
                        functionName = functionName, time = eventTime, objectReferenceParameter = objectParameter.Value,
                        messageOptions = SendMessageOptions.DontRequireReceiver
                    },
                    ParameterType.Object),
                _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter,
                    $"Can't handle EventParameter with type {typeof(T)}")
            };
        }
    }
}