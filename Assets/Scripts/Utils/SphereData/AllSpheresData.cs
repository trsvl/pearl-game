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

        public void RemoveSphere(Color color, GameObject sphere, int index)
        {
            if (allSpheres.TryGetValue(color, out List<GameObject>[] allSphere))
            {
                allSphere[index].Remove(sphere);
            }
        }

        public Dictionary<Color, List<GameObject>[]> Get()
        {
            return allSpheres;
        }
    }
}