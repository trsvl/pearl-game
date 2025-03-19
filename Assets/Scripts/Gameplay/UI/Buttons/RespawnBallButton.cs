using Gameplay.BallThrowing;
using Gameplay.SphereData;
using Gameplay.UI.Header;
using UnityEngine.UI;
using Utils.UI.Buttons;

namespace Gameplay.UI.Buttons
{
    public class RespawnBallButton : IStartGame, IAfterDestroySphereSegment, IReleaseBall, IAfterReleaseBall,
        IFinishGame, ILoseGame
    {
        private readonly Button _button;
        private readonly SpheresDictionary _spheresDictionary;
        private readonly ShotsData _shotsData;


        public RespawnBallButton(Button button, BallFactory ballFactory, SpheresDictionary spheresDictionary,
            ShotsData shotsData)
        {
            _button = button;
            _spheresDictionary = spheresDictionary;
            _shotsData = shotsData;

            _button.onClick.AddListener(ballFactory.RespawnBall);
            _button.interactable = false;
        }

        public void StartGame()
        {
            IsActiveButton(Condition());
        }

        public void OnAfterDestroySphereSegment(int currentShotsNumber)
        {
            IsActiveButton(Condition());
        }

        public void OnReleaseBall()
        {
            IsActiveButton(false);
        }

        public void OnAfterReleaseBall()
        {
            IsActiveButton(Condition());
        }

        private bool Condition()
        {
            return _spheresDictionary.GetLevelColors().Length > 2 && _shotsData.CurrentNumber > 2;
        }

        private void IsActiveButton(bool isEnabled)
        {
            _button.interactable = isEnabled;
            _button.gameObject.SetActive(isEnabled);
        }

        public void FinishGame()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void LoseGame()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}