using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay.Effects
{
    public class SpawnSmallSpheresAnimation
    {
        private const float distance = 10f;


        public IEnumerator MoveSpheresToCenter(SpheresDictionary spheresDictionary, Transform parent)
        {
            const float moveDuration = 0.2f;
            const float delayBetweenIterations = 0.1f;

            var allSpheres = spheresDictionary.GetSpheres();

            int length = allSpheres.First().Length;
            int directionX = 1;

            for (int i = 0; i < length; i++)
            {
                foreach (var sphereListArray in allSpheres)
                {
                    if (sphereListArray[i].Count <= 0) continue;

                    int localIndex = i;

                    GameObject sphereSegment = CreateSegmentObject(parent, sphereListArray[i]);
                    Vector3 initialPosition = sphereSegment.transform.position;

                    directionX *= -1;
                    sphereSegment.transform.position = GeneratePositionFromCenter(directionX, initialPosition);

                    sphereSegment.transform.DOMove(initialPosition, moveDuration).OnComplete(() =>
                    {
                        DestroySegmentObject(sphereListArray[localIndex], sphereSegment);
                    });

                    yield return new WaitForSeconds(delayBetweenIterations);
                }
            }
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

        private Vector3 GeneratePositionFromCenter(int directionX, Vector3 initialPosition)
        {
            float randomY = Random.value;

            Vector3 randomDirection =
                new Vector3(directionX * distance, randomY, initialPosition.z);
            Vector3 positionFromCenter = initialPosition + randomDirection;
            return positionFromCenter;
        }
    }
}