using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bootstrap
{
    public class Loader
    {
        private readonly GameObject _loadingScreenPrefab;
        private GameObject loadingScreen;


        public Loader(GameObject loadingScreenPrefab)
        {
            _loadingScreenPrefab = loadingScreenPrefab;
        }

        public async UniTask LoadScene(SceneName scene)
        {
            Time.timeScale = 1;
            
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

                await UniTask.Yield();
            }

            DestroyLoadingScreen();
        }

        private void CreateLoadingScreen()
        {
            if (!loadingScreen)
            {
                loadingScreen = Object.Instantiate(_loadingScreenPrefab);
            }
        }

        private void DestroyLoadingScreen()
        {
            if (loadingScreen != null)
            {
                Object.Destroy(loadingScreen.gameObject);
                loadingScreen = null;
            }
        }

        public static bool IsCurrentSceneEqual(SceneName scene)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            return currentScene == scene.ToString();
        }
    }
}