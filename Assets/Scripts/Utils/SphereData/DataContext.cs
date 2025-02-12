using System.IO;
using UnityEngine;

namespace Utils.SphereData
{
    public class DataContext
    {
        private string filePath;


        public SpheresJSON LoadSpheresJSON()
        {
            string json = File.ReadAllText(filePath);
            var spheres = JsonUtility.FromJson<SpheresJSON>(json);
            return spheres;
        }

        public void UpdateFilePath(int levelNumber)
        {
            filePath = Path.Combine(Application.streamingAssetsPath, $"Level{levelNumber}.json");
        }

        public bool CheckFileExists(int levelNumber)
        {
            var certainFilePath = Path.Combine(Application.streamingAssetsPath, $"Level{levelNumber}.json");
            return File.Exists(certainFilePath);
        }
    }
}