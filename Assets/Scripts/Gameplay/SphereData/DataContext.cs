using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Gameplay.SphereData
{
    public class DataContext
    {
        private SpheresData spheres;


        public async Task LoadSpheres(int levelNumber, SphereGenerator sphereGenerator)
        {
            string filePath = FilePath(levelNumber);

            string json;

            if (Application.platform == RuntimePlatform.Android)
            {
                using UnityWebRequest www = UnityWebRequest.Get(filePath);

                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load JSON: {www.error}");
                    return;
                }

                json = www.downloadHandler.text;
            }
            else
            {
                if (File.Exists(filePath))
                {
                    json = await File.ReadAllTextAsync(filePath);
                }
                else
                {
                    Debug.LogError($"File not found: {filePath}");
                    return;
                }
            }

            SpheresData spheresData = JsonUtility.FromJson<SpheresData>(json);
            if (spheresData == null)
            {
                Debug.LogError("Failed to parse JSON data.");
                return;
            }

            sphereGenerator.LoadSpheres(spheresData);
        }

        protected string FilePath(int level)
        {
            return Path.Combine(Application.streamingAssetsPath, $"Level{level}.json");
        }
    }
}