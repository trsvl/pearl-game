using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallThrowing;
using UnityEngine;

namespace Gameplay.Animations
{
    public class ThrowingBallAnimation : IStartAnimation, IReleaseBall
    {
        private readonly BallFactory _ballFactory;
        private readonly CameraManager _cameraManager;
        private readonly RectTransform _respawnBallButton;
        private readonly CancellationToken _cancellationToken;
        private const float startingAnimationDuration = 0.3f;


        public ThrowingBallAnimation(BallFactory ballFactory, CameraManager cameraManager,
            RectTransform respawnBallButton, CancellationToken cancellationToken)
        {
            _ballFactory = ballFactory;
            _cameraManager = cameraManager;
            _respawnBallButton = respawnBallButton;
            _cancellationToken = cancellationToken;
        }

        public async UniTask DoAnimation()
        {
            (GameObject currentBall, GameObject nextBall) = _ballFactory.InitBallsData();

            MoveBallAndUI(nextBall);

            await MoveBall(currentBall);
        }

        public async void OnReleaseBall()
        {
            while (!_ballFactory.NextBall)
            {
                if (_ballFactory.IsPreventedToSpawnBall()) return;
                await UniTask.Yield();
            }

            await _ballFactory.NextBall.transform.DOMove(_ballFactory.CurrentBallSpawnPoint, 0.2f)
                .SetEase(Ease.OutQuad)
                .ToUniTask(cancellationToken: _cancellationToken);

            _ballFactory?.SetCurrentBall();
        }

        private async UniTask MoveBall(GameObject ball)
        {
            Vector3 targetPos = ball.transform.position;
            Vector3 initialPos = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);
            ball.transform.position = initialPos;

            await ball.transform.DOMove(targetPos, startingAnimationDuration)
                .SetEase(Ease.OutQuad)
                .ToUniTask(cancellationToken: _cancellationToken);
        }

        private void MoveBallAndUI(GameObject ball)
        {
            Vector3 targetPos = ball.transform.position;
            Vector3 initialPos = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);
            ball.transform.position = initialPos;

            var updateUIPosition = new UpdateUIPosition(_cameraManager.GetMainCamera(), _respawnBallButton);
            _respawnBallButton.anchoredPosition = initialPos;
            _respawnBallButton.gameObject.SetActive(true);

            ball.transform.DOMove(targetPos, startingAnimationDuration)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() => { updateUIPosition.Update(ball.transform.position); });
        }
    }
}