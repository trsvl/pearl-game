using System;
using UnityEngine;
using Utils.EventBusSystem;

namespace Gameplay.Utils
{
    public class GameplayStateObserver
    {
        private readonly EventBus _eventBus;
        private GameplayState GameplayState = GameplayState.OFF;


        public GameplayStateObserver(EventBus eventBus)
        {
            _eventBus = eventBus;
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
                () => _eventBus.RaiseEvent<IFinishGame>(handler => handler.FinishGame()));
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