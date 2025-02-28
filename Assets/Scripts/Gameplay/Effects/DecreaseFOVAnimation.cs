using DG.Tweening;
using Gameplay.BallThrowing;
using UnityEngine;

namespace Gameplay.Effects
{
    public class DecreaseFOVAnimation
    {
        private readonly BallThrower _ballThrower;
        private readonly CameraManager _cameraManager;
        private Tweener _tweener;

        public DecreaseFOVAnimation(BallThrower ballThrower, CameraManager cameraManager)
        {
            _ballThrower = ballThrower;
            _cameraManager = cameraManager;
        }

        private void Update(float newFOV)
        {
            Vector3 newBallPosition = _cameraManager.UpdateFOV(_ballThrower._ballSize, newFOV);
            _ballThrower.UpdateData(newBallPosition);
        }

        public void Do()
        {
            const float duration = 1f;

            float initialFOV = _cameraManager.GetInitialFOV();
            float targetFOV = _cameraManager.GetNewFOV();

            _tweener = DOVirtual.Float(initialFOV, targetFOV, duration, Update)
                .OnComplete(() =>
                {
                    _tweener?.Kill();
                    _cameraManager.AssignNewFOV();
                });
        }
    }
}