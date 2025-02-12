using UnityEngine;

namespace Utils.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Debug.LogError($"There is already an instance of {typeof(T).Name}");
                Destroy(gameObject);
            }
        }
    }
}