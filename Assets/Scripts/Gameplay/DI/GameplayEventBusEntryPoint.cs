using System;
using Gameplay.Animations;
using Gameplay.BallThrowing;
using Gameplay.SphereData;
using Gameplay.UI.Header;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayEventBusEntryPoint : IInitializable, IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly IObjectResolver _container;


        public GameplayEventBusEntryPoint(EventBus eventBus, IObjectResolver container)
        {
            _eventBus = eventBus;
            _container = container;
        }

        public void Initialize()
        {
            Subscribe(
                typeof(PearlsData),
                typeof(SphereOnHitBehaviour),
                typeof(SpheresDictionary),
                typeof(ParticlesFactory)
            );
        }

        public void Dispose()
        {
            Unsubscribe(
                typeof(PearlsData),
                typeof(SphereOnHitBehaviour),
                typeof(SpheresDictionary)
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