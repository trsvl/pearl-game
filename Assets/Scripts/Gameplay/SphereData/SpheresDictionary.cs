using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Utils.EventBusSystem;
using Object = UnityEngine.Object;

namespace Gameplay.SphereData
{
    public class SpheresDictionary : IDestroySphereSegment
    {
        public Vector3 LowestSphereScale => lowestSphereScale;

        private readonly EventBus _eventBus;
        private readonly Dictionary<Color, HashSet<GameObject>[]> allSpheres = new();
        private Vector3 lowestSphereScale = Vector3.one;


        public SpheresDictionary(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

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

        private async Task DestroySpheresSegment(Color color, GameObject targetSphere)
        {
            if (!allSpheres.TryGetValue(color, out HashSet<GameObject>[] spheresLists)) return;

            foreach (HashSet<GameObject> spheresSegment in spheresLists)
            {
                if (!spheresSegment.Contains(targetSphere)) continue;

                var sortedSegmentByDistance = spheresSegment
                    .OrderBy(obj => (obj.transform.position - targetSphere.transform.position).sqrMagnitude).ToList();

                for (int i = 0; i < sortedSegmentByDistance.Count; i++)
                {
                    GameObject sphere = sortedSegmentByDistance[i];

                    spheresSegment.Remove(sphere);

                    _eventBus.RaiseEvent<IDestroySphere>(handler => handler.OnDestroySphere(sphere));

                    int delay = Mathf.RoundToInt(100 * (1f / ((i + 2) * 0.5f)));
                    await Task.Delay(delay);
                }
            }
        }

        public Dictionary<Color, HashSet<GameObject>[]>.ValueCollection GetSpheres()
        {
            return allSpheres.Values;
        }

        public void SetLowestSphereScale(Vector3 sphereScale)
        {
            if (sphereScale.x < lowestSphereScale.x)
            {
                lowestSphereScale = sphereScale;
            }
        }

        public void OnDestroySphereSegment(Color segmentColor, GameObject target)
        {
            _ = DestroySpheresSegment(segmentColor, target);
        }
    }
}