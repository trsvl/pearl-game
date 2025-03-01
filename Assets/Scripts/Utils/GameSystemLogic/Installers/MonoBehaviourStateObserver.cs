using System.Collections.Generic;
using UnityEngine;

namespace Utils.GameSystemLogic.Installers
{
    public class MonoBehaviourStateObserver : MonoBehaviour, IStateObserver, IStartGame
    {
        private readonly List<IUpdate> updateListeners = new();


        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void StartGame()
        {
            gameObject.SetActive(true);
        }

        public void AddListener(object listener)
        {
            if (listener is IUpdate concreteListener)
            {
                updateListeners.Add(concreteListener);
            }
        }

        public void RemoveListener(object listener)
        {
            if (listener is IUpdate concreteListener)
            {
                updateListeners.Remove(concreteListener);
            }
        }

        private void Update()
        {
            foreach (var listener in updateListeners)
            {
                listener.Update();
            }
        }
    }
}