using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.SphereData
{
    public class AllSpheresData
    {
        private readonly Dictionary<Color, HashSet<GameObject>[]> allSpheres = new();

        public void AddColorToDictionary(Color color, int bigSpheresCount)
        {
            if (allSpheres.ContainsKey(color)) return;
            var sphereListArray = new HashSet<GameObject>[bigSpheresCount];

            for (int i = 0; i < bigSpheresCount; i++)
            {
                sphereListArray[i] = new HashSet<GameObject>();
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

        public IEnumerator DestroySpheresSegment(Color color, GameObject targetSphere, Action<GameObject> onDestroy)
        {
            if (!allSpheres.TryGetValue(color, out HashSet<GameObject>[] spheresLists)) yield break;

            foreach (HashSet<GameObject> spheresSegment in spheresLists)
            {
                if (!spheresSegment.Contains(targetSphere)) continue;

                var sortedSegmentByDistance = spheresSegment
                    .OrderBy(obj => (obj.transform.position - targetSphere.transform.position).sqrMagnitude).ToList();

                for (int i = 0; i < sortedSegmentByDistance.Count; i++)
                {
                    GameObject sphere = sortedSegmentByDistance[i];

                    spheresSegment.Remove(sphere);
                    onDestroy?.Invoke(sphere);
                    yield return new WaitForSeconds(0.05f / ((i + 1) * 0.5f));
                }
            }

            yield return null;
        }

        public Dictionary<Color, HashSet<GameObject>[]>.ValueCollection GetSpheres()
        {
            return allSpheres.Values;
        }
    }
}