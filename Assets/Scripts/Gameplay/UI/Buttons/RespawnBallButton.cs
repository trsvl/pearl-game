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
            IsActiveButton(IsEnoughOfColors());
        }

        public void OnAfterDestroySphereSegment(int currentShotsNumber)
        {
            IsActiveButton(IsEnoughOfColors());
        }

        public void OnReleaseBall()
        {
            IsActiveButton(false);
        }

        public void OnAfterReleaseBall()
        {
            IsActiveButton(IsEnoughOfColors());
        }

        private bool IsEnoughOfColors()
        {
            return _spheresDictionary.GetLevelColors().Length > 2;
        }

        private void IsActiveButton(bool isEnabled)
        {
            _button.interactable = isEnabled;
            _button.gameObject.SetActive(isEnabled);
        }
    }
}