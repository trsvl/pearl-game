using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Gameplay.SphereData
{
    public class DataContext
    {
        private SpheresData spheres;


        public IEnumerator LoadSpheres(int levelNumber, SphereGenerator sphereGenerator)
        {
            string filePath = FilePath(levelNumber);

            string json;

            if (Application.platform == RuntimePlatform.Android)
            {
                using UnityWebRequest www = UnityWebRequest.Get(filePath);

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load JSON: {www.error}");
                    yield break;
                }

                json = www.downloadHandler.text;
            }
            else
            {
                if (File.Exists(filePath))
                {
                    json = File.ReadAllText(filePath);
                }
                else
                {
                    Debug.LogError($"File not found: {filePath}");
                    yield break;
                }
            }

            spheres = JsonUtility.FromJson<SpheresData>(json);
            if (spheres == null)
            {
                Debug.LogError("Failed to parse JSON data.");
                yield break;
            }

            sphereGenerator.LoadSpheres(spheres);
        }

        protected string FilePath(int level)
        {
            return Path.Combine(Application.streamingAssetsPath, $"Level{level}.json");
        }
    }
}