using System;
using System.Collections.Generic;

namespace Gameplay
{
    public enum ActionName
    {
        DestroySphereSegment
    }

    public interface IDestroySphereSegment
    {
        void OnDestroySphereSegment();
    }
    
    public interface IDestroySphere
    {
        void OnDestroySphere();
    }
    
    public class GameplayActionsObserver
    {
        private readonly List<object> listeners = new();


        public void AddListener(object listener)
        {
            if (listener is IDestroySphereSegment or IDestroySphere)
            {
                listeners.Add(listener);
            }
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