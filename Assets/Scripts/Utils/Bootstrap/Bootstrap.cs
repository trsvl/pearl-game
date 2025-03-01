using UnityEngine;

namespace Utils.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _installers;


        private void Awake()
        {
            foreach (var monoBehaviour in _installers)
            {
                if (monoBehaviour != null && monoBehaviour is IBootstrapInstaller bootstrapInstaller)
                {
                    bootstrapInstaller.Load();
                }
                else
                {
                    Debug.LogError($"Bootstrap Installer {monoBehaviour.GetType()} not found!");
                }
            }

            _ = Loader.Loader.Instance.LoadScene(SceneName.MainMenu);
        }
    }
}