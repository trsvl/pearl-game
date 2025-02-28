using System;
using Gameplay.BallThrowing;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay
{
    public class OnDestroySphere
    {
        private Action<GameObject> onDestroySegment;


        public OnDestroySphere(BallThrower ballThrower, CameraManager cameraManager,
            SpheresDictionary spheresDictionary, SphereOnHitBehaviour sphereOnHitBehaviour)
        {
            AddListener((_, _) =>
                ballThrower.UpdateData(cameraManager.UpdateFOV(ballThrower._BallSize)));

            AddListener((targetColor, targetSphere) =>
                spheresDictionary.DestroySpheresSegment(targetColor, targetSphere, sphereOnHitBehaviour.DestroySphere));
        }

        private void AddListener(Action<GameObject> action)
        {
            onDestroySegment += action;
        }

        public void UnsubscribeAllActions()
        {
            onDestroySegment = null;
        }

        public void Notify(GameObject sphere)
        {
            onDestroySegment?.Invoke(sphere);
        }
    }
}