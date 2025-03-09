using System.Threading.Tasks;
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

        public async Task DoAnimation()
        {
            (GameObject currentBall, GameObject nextBall) = _ballFactory.InitBallData();


            MoveBallAndUI(nextBall);

            await MoveBall(currentBall);
        }

        public async Task OnReleaseBall()
        {
            while (!_ballFactory.NextBall)
            {
                if (_ballFactory.IsPreventedToSpawnBall()) return;
                await Task.Yield();
            }

            _ballFactory.NextBall.transform.DOMove(_ballFactory.CurrentBallSpawnPoint, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _ballFactory.SetCurrentBall());
        }

        private async Task MoveBall(GameObject ball)
        {
            Vector3 targetPos = ball.transform.position;
            Vector3 initialPos = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);
            ball.transform.position = initialPos;

            var tcs = new TaskCompletionSource<bool>();

            ball.transform.DOMove(targetPos, 1f)
                .SetEase(Ease.OutQuad)
                .OnKill(() => tcs.SetResult(true));

            await tcs.Task;
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