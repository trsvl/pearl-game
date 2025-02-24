using System;
using System.Collections.Generic;

namespace Gameplay
{
    public class GameplayStateObserver
    {
        private GameplayState GameplayState = GameplayState.OFF;

        private readonly List<object> listeners = new();


        public void AddListener(object listener)
        {
            if (listener is IStartGame or IPauseGame or IResumeGame or IFinishGame or ILoseGame)
            {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(object listener)
        {
            if (listener is IStartGame or IPauseGame or IResumeGame or IFinishGame or ILoseGame)
            {
                listeners.Remove(listener);
            }
        }

        public void StartGame()
        {
            NotifyListeners<IStartGame>(GameplayState == GameplayState.OFF, GameplayState.PLAY,
                (listener) => listener.StartGame());
        }

        public void PauseGame()
        {
            NotifyListeners<IPauseGame>(GameplayState == GameplayState.PLAY, GameplayState.PAUSE,
                (listener) => listener.PauseGame());
        }

        public void ResumeGame()
        {
            NotifyListeners<IResumeGame>(GameplayState == GameplayState.PAUSE, GameplayState.PLAY,
                (listener) => listener.ResumeGame());
        }

        public void FinishGame()
        {
            NotifyListeners<IFinishGame>(true, GameplayState.FINISH,
                (listener) => listener.FinishGame());
        }

        public void LoseGame()
        {
            NotifyListeners<ILoseGame>(true, GameplayState.LOSE,
                (listener) => listener.LoseGame());
        }

        private void NotifyListeners<T>(bool condition, GameplayState newState, Action<T> action) where T : class
        {
            if (!condition) return;

            GameplayState = newState;

            foreach (var listener in listeners)
            {
                if (listener is T concreteListener)
                {
                    action(concreteListener);
                }
            }
        }
    }
}