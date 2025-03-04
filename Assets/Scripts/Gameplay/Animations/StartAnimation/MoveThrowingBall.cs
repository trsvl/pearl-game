using System.Threading.Tasks;
using DG.Tweening;
using Gameplay.BallThrowing;
using UnityEngine;

namespace Gameplay.Animations.StartAnimation
{
    public class MoveThrowingBall : IStartAnimation
    {
        private readonly BallFactory _ballFactory;


        public MoveThrowingBall(BallFactory ballFactory)
        {
            _ballFactory = ballFactory;
        }

        public async Task DoAnimation()
        {
            GameObject ball = _ballFactory.GetBall();
            Vector3 targetPos = ball.transform.position;
            ball.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y - 5f,
                ball.transform.position.z);

            var tcs = new TaskCompletionSource<bool>();

            ball.transform.DOMove(targetPos, 1f)
                .SetEase(Ease.OutQuad)
                .OnKill(() => tcs.SetResult(true));

            await tcs.Task;
        }
    }
}