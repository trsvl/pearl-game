using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallThrowing;
using Gameplay.Utils;
using UnityEngine;

namespace Gameplay.Animations
{
    public class ThrowingBallAnimation : IStartAnimation, IReleaseBall
    {
        private readonly BallFactory _ballFactory;
        private readonly MoveUIAnimation _moveUIAnimation;
        private readonly CameraManager _cameraManager;
        private readonly RectTransform _respawnBallButton;


        public ThrowingBallAnimation(BallFactory ballFactory, MoveUIAnimation moveUIAnimation,
            CameraManager cameraManager, RectTransform respawnBallButton)
        {
            _ballFactory = ballFactory;
            _moveUIAnimation = moveUIAnimation;
            _cameraManager = cameraManager;
            _respawnBallButton = respawnBallButton;
        }

        public async UniTask DoAnimation()
        {
            (GameObject currentBall, GameObject nextBall) = _ballFactory.InitBallData();


            MoveBallAndUI(nextBall);

            await MoveBall(currentBall);
        }

        public async UniTask OnReleaseBall()
        {
            while (!_ballFactory.NextBall)
            {
                if (_ballFactory.IsPreventedToSpawnBall()) return;
                await UniTask.Yield();
            }

            await _ballFactory.NextBall.transform.DOMove(_ballFactory.CurrentBallSpawnPoint, 0.5f)
                .SetEase(Ease.OutQuad)
                .ToUniTask();

            _ballFactory.SetCurrentBall();
        }

        private async UniTask MoveBall(GameObject ball)
        {
            Vector3 targetPos = ball.transform.position;
            Vector3 initialPos = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);
            ball.transform.position = initialPos;

            await ball.transform.DOMove(targetPos, 1f)
                .SetEase(Ease.OutQuad)
                .ToUniTask();
        }

        private void MoveBallAndUI(GameObject ball)
        {
            Vector3 targetPos = ball.transform.position;
            Vector3 initialPos = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);
            ball.transform.position = initialPos;

            var updateUIPosition = new UpdateUIPosition(_cameraManager.GetMainCamera(), _respawnBallButton);
            _respawnBallButton.anchoredPosition = initialPos;

            ball.transform.DOMove(targetPos, 1f)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() => { updateUIPosition.Update(ball.transform.position); });
        }
    }
}