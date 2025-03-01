using System;
using System.Collections.Generic;
using Gameplay.BallThrowing;
using UnityEngine;

namespace Gameplay.Actions
{
    public class OnDestroySphereSegment
    {
        private Action<Color, GameObject> action;
        private readonly List<IDestroySphereSegment> listeners = new();


        public void AddListener(IDestroySphereSegment listener)
        {
            listeners.Add(listener);
        }

        public void SubscribeEvent(Action<Color, GameObject> newAction)
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

        public void NotifyAll(Color color, GameObject targetSphere)
        {
            foreach (var listener in listeners)
            {
                listener.OnDestroySphereSegment();
            }

            action?.Invoke(color, targetSphere);
        }
    }
}