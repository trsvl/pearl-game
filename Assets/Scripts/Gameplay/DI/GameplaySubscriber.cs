using System;
using Gameplay.Animations;
using Gameplay.BallThrowing;
using Gameplay.SphereData;
using Gameplay.UI.Buttons;
using Gameplay.UI.Header;
using Gameplay.UI.Popup;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplaySubscriber : IInitializable, IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly GameplayStateObserver _gameplayStateObserver;
        private readonly IObjectResolver _container;


        public GameplaySubscriber(EventBus eventBus, GameplayStateObserver gameplayStateObserver,
            IObjectResolver container)
        {
            _eventBus = eventBus;
            _gameplayStateObserver = gameplayStateObserver;
            _container = container;
        }

        public void Initialize()
        {
            AddGameplayListeners(
                typeof(BallThrower),
                typeof(SphereGenerator),
                typeof(PauseButton),
                typeof(RespawnBallButton),
                typeof(GamePopupManager)
            );

            Subscribe(
                typeof(PearlsData),
                typeof(SphereOnHitBehaviour),
                typeof(SpheresDictionary),
                typeof(ParticlesFactory),
                typeof(ThrowingBallAnimation),
                typeof(RespawnBallButton)
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

        private void AddGameplayListeners(params Type[] types)
        {
            foreach (var type in types)
            {
                var handler = _container.Resolve(type);
                _gameplayStateObserver.AddListener(handler);
            }
        }
    }
}