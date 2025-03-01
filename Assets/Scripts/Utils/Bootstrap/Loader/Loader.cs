using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace Utils.Bootstrap.Loader
{
    public class Loader : Singleton<Loader>
    {
        private GameObject _loadingScreenPrefab;
        private GameObject loadingScreen;


        public void Init(GameObject loadingScreenPrefab)
        {
            _loadingScreenPrefab = loadingScreenPrefab;
        }

        public async Task LoadScene(SceneName scene)
        {
            CreateLoadingScreen();

            var sceneName = scene.ToString();
            
            AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName);

            if (loadingScene == null)
            {
                Debug.LogError($"Failed to load scene {sceneName}");
                return;
            }

            loadingScene.allowSceneActivation = false;

            while (!loadingScene.isDone)
            {
                if (loadingScene.progress >= 0.9f)
                {
                    loadingScene.allowSceneActivation = true;
                }

                await Task.Yield();
            }

            DestroyLoadingScreen();
        }

        public void CreateLoadingScreen()
        {
            if (!loadingScreen)
            {
                loadingScreen = Instantiate(_loadingScreenPrefab);
            }
        }

        public void DestroyLoadingScreen()
        {
            Destroy(loadingScreen.gameObject);
        }
    }
}