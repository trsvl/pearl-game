using System.IO;
using Gameplay.SphereData;
using UnityEngine;

namespace Dev.LevelBuilder
{
    public class DataContextBuilder : DataContext
    {
        public void SaveSpheresDataToJSON(SpheresData spheresData, int levelNumber)
        {
            string json = JsonUtility.ToJson(spheresData, true);
            Debug.Log(json);
            File.WriteAllText(FilePath(levelNumber), json);
        }

        public bool CheckFileExists(int levelNumber)
        {
            return File.Exists(FilePath(levelNumber));
        }

        public void DeleteFile(int levelNumber)
        {
            File.Delete(FilePath(levelNumber));
        }
    }
}