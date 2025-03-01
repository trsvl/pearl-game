using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Actions
{
    public class OnDestroySphere
    {
        private Action<GameObject> action;
        private readonly List<IDestroySphere> listeners = new();


        public void AddListener(IDestroySphere listener)
        {
            listeners.Add(listener);
        }

        public void SubscribeEvent(Action<GameObject> newAction)
        {
            action += newAction;
        }

        public void RemoveAllListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners.RemoveAt(i);
            }

            action = null;
        }

        public void NotifyAll(GameObject sphere)
        {
            foreach (var listener in listeners)
            {
                listener.OnDestroySphere();
            }

            action?.Invoke(sphere);
        }
    }
}