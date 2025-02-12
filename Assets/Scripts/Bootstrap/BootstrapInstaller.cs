using UnityEngine;
using Utils.Loader;
using Utils.PlayerData;

namespace Bootstrap
{
    public class BootstrapInstaller : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreenPrefab;

        private void Start()
        {
            var loader = new GameObject().AddComponent<Loader>();
            loader.gameObject.name = "Loader";
            loader.Init(loadingScreenPrefab);

            var playerData = new GameObject().AddComponent<PlayerData>();
            playerData.gameObject.name = "PlayerData";
            playerData.Init();

            _ = loader.LoadScene(SceneName.MainMenu);
        }
    }
}