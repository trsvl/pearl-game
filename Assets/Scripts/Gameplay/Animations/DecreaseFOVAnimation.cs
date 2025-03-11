using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallThrowing;
using UnityEngine;
using Utils.EventBusSystem;

namespace Gameplay.Animations
{
    public class DecreaseFOVAnimation : IDestroySphereLayer
    {
        private readonly BallFactory _ballFactory;
        private readonly CameraManager _cameraManager;
        private readonly EventBus _eventBus;
        private readonly CancellationToken _cancellationToken;


        public DecreaseFOVAnimation(BallFactory ballFactory, CameraManager cameraManager, EventBus eventBus,
            CancellationToken cancellationToken)
        {
            _ballFactory = ballFactory;
            _cameraManager = cameraManager;
            _eventBus = eventBus;
            _cancellationToken = cancellationToken;
        }

        public void OnDestroySphereLayer(int destroyedSphereLayers)
        {
            _ = DoAnimation(destroyedSphereLayers);
        }

        private async UniTask DoAnimation(int destroyedSphereLayers)
        {
            const float duration = 1f;

            await UniTask.Delay(500, cancellationToken: _cancellationToken);

            float initialFOV = _cameraManager.GetInitialFOV();
            float targetFOV = _cameraManager.CalculateNewFOV(destroyedSphereLayers);
           // _eventBus.RaiseEvent<IChangeFOVAnimation>(handler => handler.OnStartChangeFOV());

            await DOVirtual.Float(initialFOV, targetFOV, duration, _ballFactory.UpdateBallsPosition).ToUniTask(cancellationToken: _cancellationToken);

            _cameraManager.AssignNewFOV();
           // _eventBus.RaiseEvent<IChangeFOVAnimation>(handler => handler.OnEndChangeFOV());
        }
    }
}