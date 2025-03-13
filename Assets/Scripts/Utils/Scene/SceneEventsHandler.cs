using System;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;

namespace Utils.Scene
{
    public abstract class SceneEventsHandler : IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly IObjectResolver _container;
        private readonly Type[] _subscribers;


        protected SceneEventsHandler(EventBus eventBus, IObjectResolver container, Type[] subscribers)
        {
            _eventBus = eventBus;
            _container = container;
            _subscribers = subscribers;

            Subscribe(
                _subscribers
            );
        }

        public void Dispose()
        {
            Unsubscribe(
                _subscribers
            );
        }

        private void Subscribe(params Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(IGlobalSubscriber).IsAssignableFrom(type))
                {
                    var handler = _container.Resolve(type) as IGlobalSubscriber;
                    _eventBus.Subscribe(handler);
                }
                else
                {
                    Debug.LogError($"Type {type.Name} does not implement IGlobalSubscriber");
                }
            }
        }

        private void Unsubscribe(params Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(IGlobalSubscriber).IsAssignableFrom(type))
                {
                    var handler = _container.Resolve(type) as IGlobalSubscriber;
                    _eventBus.Unsubscribe(handler);
                }
                else
                {
                    Debug.LogError($"Type {type.Name} does not implement IGlobalSubscriber");
                }
            }
        }
    }
}