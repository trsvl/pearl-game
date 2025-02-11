using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Generator
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

        public void SaveSpheresJSON(
            SphereGenerator generator,
            string[] newMaterialNames,
            List<int[]> materialIndexesList)
        {
            var spheresJson = new SpheresJSON
            {
                isStaticRadius = generator._isStaticSize,
                smallSphereRadius = generator._isStaticSize
                    ? generator._sphereLocalScaleRadius
                    : 0,
                smallSphereRadiusScale = generator._smallSphereRadiusScaleRuntime,
                materialNames = newMaterialNames,
                spheres = new SphereJSON[generator._spheres.Length]
            };

            for (var i = 0; i < generator._spheres.Length; i++)
            {
                spheresJson.spheres[i] = new SphereJSON
                {
                    smallSphereCount = generator._spheres[i].smallSphereCount,
                    largeSphereRadius = generator._spheres[i].largeSphereRadius,
                    materialIndexes = materialIndexesList[i]
                };
            }

            string json = JsonUtility.ToJson(spheresJson, true);
            Debug.Log(json);
            File.WriteAllText(filePath, json);
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

        public void DeleteFile()
        {
            File.Delete(filePath);
        }
    }
}