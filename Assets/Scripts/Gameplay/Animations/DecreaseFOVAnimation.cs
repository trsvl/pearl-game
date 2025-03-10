﻿using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallThrowing;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;

namespace Gameplay.Animations
{
    public class DecreaseFOVAnimation : IAnimation //!!!
    {
        private readonly BallFactory _ballFactory;
        private readonly CameraManager _cameraManager;
        private readonly EventBus _eventBus;
        private Tweener _tweener;


        public DecreaseFOVAnimation(BallFactory ballFactory, CameraManager cameraManager, EventBus eventBus)
        {
            _ballFactory = ballFactory;
            _cameraManager = cameraManager;
            _eventBus = eventBus;
        }

        public async UniTask DoAnimation()
        {
            const float duration = 1f;

            float initialFOV = _cameraManager.GetInitialFOV();
            float targetFOV = _cameraManager.GetNewFOV();
            _eventBus.RaiseEvent<IChangeFOVAnimation>(handler => handler.OnStartChangeFOV());

            _tweener = DOVirtual.Float(initialFOV, targetFOV, duration, _ballFactory.UpdateBallPosition);

            await _tweener.AsyncWaitForCompletion();

            _tweener?.Kill();
            _cameraManager.AssignNewFOV();
            _eventBus.RaiseEvent<IChangeFOVAnimation>(handler => handler.OnEndChangeFOV());
        }
    }
}