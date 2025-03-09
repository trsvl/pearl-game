using System.Threading.Tasks;
using Gameplay.BallThrowing;
using Gameplay.SphereData;
using UnityEngine.UI;

namespace Gameplay.UI.Buttons
{
    public class RespawnBallButton : IStartGame, IAfterDestroySphereSegment, IReleaseBall, IAfterReleaseBall
    {
        private readonly Button _button;
        private readonly SpheresDictionary _spheresDictionary;


        public RespawnBallButton(Button button, BallFactory ballFactory, SpheresDictionary spheresDictionary)
        {
            _button = button;
            _spheresDictionary = spheresDictionary;

            _button.onClick.AddListener(ballFactory.RespawnBall);
            _button.interactable = false;
        }

        public void StartGame()
        {
            _button.interactable = true;
        }

        public void OnAfterDestroySphereSegment()
        {
            _button.interactable = IsEnoughOfColors();
        }

        public Task OnReleaseBall()
        {
            _button.interactable = false;
            return Task.CompletedTask;
        }

        public void OnAfterReleaseBall()
        {
            _button.interactable = IsEnoughOfColors();
        }

        private bool IsEnoughOfColors()
        {
            return _spheresDictionary.GetLevelColors().Length > 1;
        }
    }
}