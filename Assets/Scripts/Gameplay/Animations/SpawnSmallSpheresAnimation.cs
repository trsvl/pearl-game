using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.SphereData;
using UnityEngine;
using Utils.EventBusSystem;

namespace Gameplay.Animations
{
    public class SpawnSmallSpheresAnimation : IStartAnimation
    {
        private readonly SpheresDictionary _spheresDictionary;
        private readonly Transform _parent;
        private readonly EventBus _eventBus;


        public SpawnSmallSpheresAnimation(SpheresDictionary spheresDictionary, Transform parent, EventBus eventBus)
        {
            _spheresDictionary = spheresDictionary;
            _parent = parent;
            _eventBus = eventBus;
        }

        private GameObject CreateSegmentObject(Transform parent, HashSet<GameObject> allSpheres)
        {
            GameObject sphereSegment = new GameObject();
            sphereSegment.transform.SetParent(parent, false);

            foreach (var sphere in allSpheres)
            {
                sphere.transform.SetParent(sphereSegment.transform, false);
                sphere.SetActive(true);
            }

            return sphereSegment;
        }

        private void DestroySegmentObject(HashSet<GameObject> allSpheres, GameObject sphereSegment)
        {
            foreach (var sphere in allSpheres)
            {
                sphere.transform.SetParent(sphereSegment.transform.parent);
            }

            Object.Destroy(sphereSegment);
        }

        public async UniTask DoAnimation()
        {
            const float moveDuration = 0.05f;
            const int delayBetweenIterations = (int)(moveDuration * 1000);

            var allSpheres = _spheresDictionary.GetSpheres();

            int length = allSpheres.First().Length;

            for (int i = 0; i < length; i++)
            {
                foreach (var sphereListArray in allSpheres)
                {
                    if (sphereListArray[i].Count <= 0) continue;

                    int localIndex = i;

                    GameObject sphereSegment = CreateSegmentObject(_parent, sphereListArray[i]);
                    Vector3 initialScale = sphereSegment.transform.localScale;

                    await sphereSegment.transform.DOScale(initialScale * 1.2f, moveDuration).SetEase(Ease.OutBack)
                        .ToUniTask();

                    UniTask scaleSphereSegment = sphereSegment.transform.DOScale(initialScale, moveDuration)
                        .SetEase(Ease.OutBack).ToUniTask();

                    await scaleSphereSegment.ContinueWith(() =>
                        DestroySegmentObject(sphereListArray[localIndex], sphereSegment));

                    _eventBus.RaiseEvent<ISpawnSphereSegment>(handler => handler.OnSpawnSphereSegment());

                    await UniTask.Delay(delayBetweenIterations);
                }
            }
        }
    }
}