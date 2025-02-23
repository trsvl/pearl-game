using System.Collections.Generic;
using UnityEngine;

namespace Utils.SphereData
{
    public class AllSpheresData
    {
        private readonly Dictionary<Color, List<GameObject>[]> allSpheres = new();

        public void AddColorToDictionary(Color color, int bigSpheresCount)
        {
            if (allSpheres.ContainsKey(color)) return;

            var sphereListArray = new List<GameObject>[bigSpheresCount];

            for (int i = 0; i < bigSpheresCount; i++)
            {
                sphereListArray[i] = new List<GameObject>();
            }

            allSpheres.Add(color, sphereListArray);
        }

        public void AddSphere(Color color, GameObject sphere, int index)
        {
            allSpheres[color][index].Add(sphere);
        }

        public void DestroyAllSpheres()
        {
            foreach (var pair in allSpheres)
            {
                foreach (var sphereList in pair.Value)
                {
                    foreach (var sphere in sphereList)
                    {
                        if (sphere)
                        {
                            Object.Destroy(sphere);
                        }
                    }
                }
            }

            allSpheres.Clear();
        }

        public Dictionary<Color, List<GameObject>[]> Get() //!!!
        {
            return allSpheres;
        }
    }
}