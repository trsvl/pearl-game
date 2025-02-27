using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay.Animations
{
    public class ChangeCameraFOV
    {
        private int alreadyTriggeredIndex;


        public void Do(SpheresDictionary spheresDictionary)
        {
            var allSpheres = spheresDictionary.GetSpheres();

            int length = allSpheres.First().Length;

            for (int i = length - 1; i >= 0; i--)
            {
                bool isSphereSegmentDestroyed = true;

                foreach (var sphereListArray in allSpheres)
                {
                    if (sphereListArray[i].Count == 0 && alreadyTriggeredIndex > i) continue;

                    isSphereSegmentDestroyed = false;
                    break;
                }

                if (isSphereSegmentDestroyed)
                {
                    ReduceFOV();
                }
                else
                {
                    break;
                }
            }
        }

        private void ReduceFOV()
        {
        }
    }
}