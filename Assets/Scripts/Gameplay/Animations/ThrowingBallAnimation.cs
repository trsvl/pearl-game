using System.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallThrowing;
using UnityEngine;

namespace Gameplay.Animations
{
    public class ThrowingBallAnimation : IStartAnimation, IReleaseBall
    {
        private readonly BallFactory _ballFactory;


        public ThrowingBallAnimation(BallFactory ballFactory)
        {
            _ballFactory = ballFactory;
        }

        public async Task DoAnimation()
        {
            (GameObject currentBall, GameObject nextBall) = _ballFactory.InitBallData();

            _ = MoveBall(nextBall);
            await MoveBall(currentBall);
        }

        private async Task MoveBall(GameObject ball)
        {
            Vector3 targetPos = ball.transform.position;
            ball.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);

            var tcs = new TaskCompletionSource<bool>();

            ball.transform.DOMove(targetPos, 1f)
                .SetEase(Ease.OutQuad)
                .OnKill(() => tcs.SetResult(true));

            await tcs.Task;
        }

        public void OnReleaseBall(GameObject nextBall)
        {
            if (!nextBall) return;

            nextBall.transform.DOMove(_ballFactory.CurrentBallSpawnPoint, 1f)
                .SetEase(Ease.OutQuad).OnComplete(() => { _ballFactory.SetCurrentBall(); });
        }
    }
}