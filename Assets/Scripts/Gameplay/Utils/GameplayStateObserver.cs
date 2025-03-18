using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.EventBusSystem;

namespace Gameplay.Utils
{
    public class GameplayStateObserver
    {
        private readonly EventBus _eventBus;
        private readonly FinishGameController _finishGameController;
        private GameplayState GameplayState = GameplayState.OFF;


        public GameplayStateObserver(EventBus eventBus, FinishGameController finishGameController)
        {
            _eventBus = eventBus;
            _finishGameController = finishGameController;
        }

        public void StartGame()
        {
            Time.timeScale = 1;

            NotifyListeners(GameplayState == GameplayState.OFF, GameplayState.PLAY,
                () => _eventBus.RaiseEvent<IStartGame>(handler => handler.StartGame()));
        }

        public void PauseGame()
        {
            Time.timeScale = 0;

            NotifyListeners(GameplayState == GameplayState.PLAY, GameplayState.PAUSE,
                () => _eventBus.RaiseEvent<IPauseGame>(handler => handler.PauseGame()));
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;

            NotifyListeners(GameplayState == GameplayState.PAUSE, GameplayState.PLAY,
                () => _eventBus.RaiseEvent<IResumeGame>(handler => handler.ResumeGame()));
        }

        public void FinishGame()
        {
            Time.timeScale = 0;

            NotifyListeners(GameplayState == GameplayState.PLAY, GameplayState.FINISH,
                () =>
                {
                    _eventBus.RaiseEvent<IFinishGame>(handler => handler.FinishGame());
                    _finishGameController.FinishGame().Forget();
                });
        }

        public void LoseGame()
        {
            Time.timeScale = 0;

            NotifyListeners(GameplayState == GameplayState.PLAY, GameplayState.LOSE,
                () => _eventBus.RaiseEvent<ILoseGame>(handler => handler.LoseGame()));
        }

        private void NotifyListeners(bool condition, GameplayState newState, Action action)
        {
            if (!condition) return;

            GameplayState = newState;

            action?.Invoke();
        }
    }
}