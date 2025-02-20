using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils.SphereData;

namespace Dev.LevelBuilder
{
    public class DataContextBuilder : DataContext
    {
        public void SaveSpheresJSON(
            SphereGeneratorBuilder generatorBuilder,
            string[] newColorNames,
            List<int[]> colorIndexesList,
            int levelNumber)
        {
            var spheresJson = new SpheresJSON
            {
                smallSphereRadius = generatorBuilder._smallSphereRadius,
                smallSphereScale = generatorBuilder._smallSphereScale,
                colorNames = newColorNames,
                spheres = new SphereJSON[generatorBuilder._bigSpheres.Length]
            };

            for (var i = 0; i < generatorBuilder._bigSpheres.Length; i++)
            {
                spheresJson.spheres[i] = new SphereJSON
                {
                    smallSphereCount = generatorBuilder._bigSpheres[i]._smallSphereCount,
                    largeSphereRadius = generatorBuilder._bigSpheres[i]._largeSphereRadius,
                    colorIndexes = colorIndexesList[i]
                };
            }

            string json = JsonUtility.ToJson(spheresJson, true);
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