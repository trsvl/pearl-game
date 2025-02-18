using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils.SphereData;

namespace LevelGenerator
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
            SphereGeneratorBuilder generatorBuilder,
            string[] newMaterialNames,
            List<int[]> materialIndexesList)
        {
            var spheresJson = new SpheresJSON
            {
                isStaticRadius = generatorBuilder._isStaticSize,
                smallSphereRadius = generatorBuilder._isStaticSize
                    ? generatorBuilder._sphereLocalScaleRadius
                    : 0,
                smallSphereRadiusScale = generatorBuilder._smallSphereRadiusScaleRuntime,
                colorNames = newMaterialNames,
                spheres = new SphereJSON[generatorBuilder._spheres.Length]
            };

            for (var i = 0; i < generatorBuilder._spheres.Length; i++)
            {
                spheresJson.spheres[i] = new SphereJSON
                {
                    smallSphereCount = generatorBuilder._spheres[i].smallSphereCount,
                    largeSphereRadius = generatorBuilder._spheres[i].largeSphereRadius,
                    colorIndexes = materialIndexesList[i]
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