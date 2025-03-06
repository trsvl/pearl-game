using Gameplay.BallThrowing;
using UnityEngine.UI;

namespace Gameplay.UI.Buttons
{
    public class RespawnBallButton : IStartGame
    {
        private readonly Button _button;


        public RespawnBallButton(Button button, BallFactory ballFactory)
        {
            _button = button;

            _button.onClick.AddListener(ballFactory.RespawnBall);
            _button.enabled = false;
        }

        public void StartGame()
        {
            _button.enabled = true;
        }
    }
}