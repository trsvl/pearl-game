using System.Collections.Generic;
using UnityEngine;

namespace Utils.SphereData
{
    public class AllSpheres
    {
        private readonly Dictionary<Color, List<Vector3>[]> allSpheres = new();

        public void AddBigSphereToDictionary(Color color, int bigSpheresCount)
        {
            var sphereListArray = new List<Vector3>[bigSpheresCount];

            for (int i = 0; i < bigSpheresCount; i++)
            {
                sphereListArray[i] = new List<Vector3>();
            }

            allSpheres.Add(color, sphereListArray);
        }

        public void AddSphere(Color color, Vector3 sphereLocalPosition, int index)
        {
            allSpheres[color][index].Add(sphereLocalPosition);
        }

        public Dictionary<Color, List<Vector3>[]> Get()
        {
            return allSpheres;
        }
    }
}