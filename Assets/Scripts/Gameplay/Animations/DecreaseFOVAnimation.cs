using System.Threading;
using Bootstrap;
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
        private readonly CameraController _cameraController;
        private readonly EventBus _eventBus;
        private readonly CancellationToken _cancellationToken;


        public DecreaseFOVAnimation(BallFactory ballFactory, CameraController cameraController, EventBus eventBus,
            CancellationToken cancellationToken)
        {
            _ballFactory = ballFactory;
            _cameraController = cameraController;
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

            float initialFOV = _cameraController.GetInitialFOV();
            float targetFOV = _cameraController.CalculateNewFOV(destroyedSphereLayers);
           // _eventBus.RaiseEvent<IChangeFOVAnimation>(handler => handler.OnStartChangeFOV());

            await DOVirtual.Float(initialFOV, targetFOV, duration, _ballFactory.UpdateBallsPosition).ToUniTask(cancellationToken: _cancellationToken);

            _cameraController.AssignNewFOV();
           // _eventBus.RaiseEvent<IChangeFOVAnimation>(handler => handler.OnEndChangeFOV());
        }
    }
}