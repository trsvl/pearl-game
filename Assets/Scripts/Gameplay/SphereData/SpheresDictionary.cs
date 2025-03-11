using System;
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
        private readonly EventBus _eventBus;
        private readonly CancellationToken _cancellationToken;
        private readonly Dictionary<Color, HashSet<GameObject>[]> _allSpheres = new();
        private List<Color> _levelColors;
        private int _sphereLayers = -1;
        private int _ignoreIndex = -1;
        private GameObject _targetSphere;

        private readonly Queue<(Color, GameObject, int)> _queue = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);


        public SpheresDictionary(EventBus eventBus, CancellationToken cancellationToken = default)
        {
            _eventBus = eventBus;
            _cancellationToken = cancellationToken;
        }

        public void AddColorToDictionary(Color color, int bigSpheresCount)
        {
            if (_allSpheres.ContainsKey(color)) return;

            var sphereListArray = new HashSet<GameObject>[bigSpheresCount];

            for (int i = 0; i < bigSpheresCount; i++)
            {
                sphereListArray[i] = new HashSet<GameObject>();
            }

            _allSpheres.Add(color, sphereListArray);
        }

        public void AddSphere(Color color, GameObject sphere, int index)
        {
            _allSpheres[color][index].Add(sphere);
        }

        public void DestroyAllSpheres()
        {
            foreach (var pair in _allSpheres)
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

            _allSpheres.Clear();
        }

        private async UniTask DestroySpheresSegment(Color color, GameObject targetSphere, int currentShotsNumber)
        {
            _targetSphere = targetSphere;

            foreach (Color key in _allSpheres.Keys)
            {
                if (!ColorsAreSimilar(key, color)) continue;

                foreach (HashSet<GameObject> spheresSegment in _allSpheres[key])
                {
                    if (!spheresSegment.Contains(_targetSphere)) continue;

                    await DestroySphereSegment(spheresSegment, _targetSphere);
                }
            }

            await CheckSphereLayers();

            CheckDictionaryColors();

            _eventBus.RaiseEvent<IAfterDestroySphereSegment>(handler =>
                handler.OnAfterDestroySphereSegment(currentShotsNumber));
        }

        private async UniTask CheckSphereLayers()
        {
            var sphereValues = _allSpheres.Values;
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
                        continue;
                    }

                    if (sphereListArray[i].Count > 0 || (_ignoreIndex != -1 && _ignoreIndex <= i)) break;

                    count++;

                    if (count != sphereValues.Count) continue;

                    _sphereLayers = GetSphereLayersCount(i, length);
                    _ignoreIndex = i;
                    isDestroyLayer = true;
                    _eventBus.RaiseEvent<IDestroySphereLayer>(handler => handler.OnDestroySphereLayer(_sphereLayers));
                }
            }
        }

        private int GetSphereLayersCount(int i, int length)
        {
            return Math.Abs(i - length) - (_sphereLayers == -1 ? 0 : _sphereLayers);
        }

        private void CheckDictionaryColors()
        {
            List<Color> colors = new List<Color>();

            foreach (Color key in _allSpheres.Keys)
            {
                int count = 0;

                foreach (HashSet<GameObject> spheresSegment in _allSpheres[key])
                {
                    if (spheresSegment.Count > 0) break;

                    count++;

                    if (count == _allSpheres[key].Length)
                    {
                        colors.Add(key);
                    }
                }
            }

            foreach (Color color in colors)
            {
                _allSpheres.Remove(color);
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
            return _allSpheres.Values;
        }

        public void OnDestroySphereSegment(Color segmentColor, GameObject target, int currentShotsNumber)
        {
            _queue.Enqueue((segmentColor, target, currentShotsNumber));
            ProcessQueue().Forget();
        }

        public Color[] GetLevelColors()
        {
            return _allSpheres.Keys.ToArray();
        }

        private async UniTaskVoid ProcessQueue()
        {
            await _semaphore.WaitAsync(_cancellationToken);

            try
            {
                while (_queue.Count > 0)
                {
                    var (color, targetSphere, shots) = _queue.Dequeue();
                    await DestroySpheresSegment(color, targetSphere, shots);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}