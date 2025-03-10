using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Utils
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
            else
            {
                Debug.LogError($"{listener} is not inherit from GameplayStateObserver");
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
            Time.timeScale = 1;
            
            NotifyListeners<IStartGame>(GameplayState == GameplayState.OFF, GameplayState.PLAY,
                (listener) => listener.StartGame());
        }

        public void PauseGame()
        {
            Time.timeScale = 0;

            NotifyListeners<IPauseGame>(GameplayState == GameplayState.PLAY, GameplayState.PAUSE,
                (listener) => listener.PauseGame());
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;

            NotifyListeners<IResumeGame>(GameplayState == GameplayState.PAUSE, GameplayState.PLAY,
                (listener) => listener.ResumeGame());
        }

        public void FinishGame()
        {
            Time.timeScale = 0;
            
            NotifyListeners<IFinishGame>(GameplayState == GameplayState.PLAY, GameplayState.FINISH,
                (listener) => listener.FinishGame());
        }

        public void LoseGame()
        {
            Time.timeScale = 0;
            
            NotifyListeners<ILoseGame>(GameplayState == GameplayState.PLAY, GameplayState.LOSE,
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