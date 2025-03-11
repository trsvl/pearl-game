using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.EventBusSystem;
using Object = UnityEngine.Object;

namespace Gameplay.SphereData
{
    public class SpheresDictionary : IDestroySphereSegment
    {
        public Vector3 LowestSphereScale => lowestSphereScale;

        private readonly EventBus _eventBus;
        private readonly CancellationToken _cancellationToken;
        private readonly Dictionary<Color, HashSet<GameObject>[]> allSpheres = new();
        private Vector3 lowestSphereScale = Vector3.one * 10f;
        private List<Color> _levelColors;
        private int sphereLayers = -1;
        private GameObject _targetSphere;


        public SpheresDictionary(EventBus eventBus, CancellationToken cancellationToken = default)
        {
            _eventBus = eventBus;
            _cancellationToken = cancellationToken;
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

        private async UniTask DestroySpheresSegment(Color color, GameObject targetSphere)
        {
            _targetSphere = targetSphere;

            foreach (Color key in allSpheres.Keys)
            {
                if (!ColorsAreSimilar(key, color)) continue;

                foreach (HashSet<GameObject> spheresSegment in allSpheres[key])
                {
                    if (!spheresSegment.Contains(_targetSphere)) continue;

                    await DestroySphereSegment(spheresSegment, _targetSphere);
                }
            }

            await CheckSphereLayers();

            CheckDictionaryColors();

            _eventBus.RaiseEvent<IAfterDestroySphereSegment>(handler =>
                handler.OnAfterDestroySphereSegment());
        }

        private async UniTask CheckSphereLayers()
        {
            var sphereValues = allSpheres.Values;
            int length = sphereValues.First().Length;

            bool isDestroyLayer = false;

            for (int i = 0; i < length; i++)
            {
                int count = 0;

                foreach (var sphereListArray in sphereValues)
                {
                    if (isDestroyLayer)
                    {
                        await DestroySphereSegment(sphereListArray[i], _targetSphere);
                    }

                    else if (sphereListArray[i].Count > 0 || sphereLayers == i) break;

                    if (isDestroyLayer) continue;

                    count++;

                    if (count != sphereValues.Count) continue;

                    sphereLayers = i;
                    isDestroyLayer = true;
                }
            }
        }

        private void CheckDictionaryColors()
        {
            List<Color> colors = new List<Color>();

            foreach (Color key in allSpheres.Keys)
            {
                int count = 0;

                foreach (HashSet<GameObject> spheresSegment in allSpheres[key])
                {
                    if (spheresSegment.Count > 0) break;

                    count++;

                    if (count == allSpheres[key].Length)
                    {
                        colors.Add(key);
                    }
                }
            }

            foreach (Color color in colors)
            {
                allSpheres.Remove(color);
            }
        }

        private async UniTask DestroySphereSegment(HashSet<GameObject> spheresSegment, GameObject targetSphere)
        {
            var sortedSegmentByDistance = spheresSegment
                .OrderBy(obj => (obj.transform.position - targetSphere.transform.position).sqrMagnitude)
                .ToList();

            for (int i = 0; i < sortedSegmentByDistance.Count; i++)
            {
                GameObject sphere = sortedSegmentByDistance[i];

                spheresSegment.Remove(sphere);

                _eventBus.RaiseEvent<IDestroySphere>(handler => handler.OnDestroySphere(sphere));

                int delay = Mathf.RoundToInt(100 * (1f / ((i + 2) * 0.5f)));
                await UniTask.Delay(delay, cancellationToken: _cancellationToken);
            }
        }

        private bool ColorsAreSimilar(Color color1, Color color2, float tolerance = 0.05f)
        {
            return Mathf.Abs(color1.r - color2.r) < tolerance &&
                   Mathf.Abs(color1.g - color2.g) < tolerance &&
                   Mathf.Abs(color1.b - color2.b) < tolerance &&
                   Mathf.Abs(color1.a - color2.a) < tolerance;
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

        public Color[] GetLevelColors()
        {
            return allSpheres.Keys.ToArray();
        }
    }
}