using UnityEngine;

namespace Utils.Bootstrap.Loader
{
    public class LoaderInstaller : MonoBehaviour, IBootstrapInstaller
    {
        [SerializeField] private GameObject loadingScreenPrefab;


        public void Load()
        {
            Loader loader = new GameObject().AddComponent<Loader>();
            loader.gameObject.name = "Loader";
            loader.Init(loadingScreenPrefab);
        }
    }
}