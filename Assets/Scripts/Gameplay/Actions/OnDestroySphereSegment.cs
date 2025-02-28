using System;
using Gameplay.BallThrowing;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay.Actions
{
    public class OnDestroySphereSegment
    {
        private Action<Color, GameObject> onDestroySegment;


        public OnDestroySphereSegment(BallThrower ballThrower, CameraManager cameraManager,
            SpheresDictionary spheresDictionary, OnDestroySphere onDestroySphere)
        {
            AddListener((_, _) =>

            AddListener((targetColor, targetSphere) =>
                spheresDictionary.DestroySpheresSegment(targetColor, targetSphere, onDestroySphere.Notify));
        }

        private void AddListener(Action<Color, GameObject> action)
        {
            onDestroySegment += action;
        }

        public void UnsubscribeAllActions()
        {
            onDestroySegment = null;
        }

        public void DestroySegment(Color color, GameObject target)
        {
            onDestroySegment?.Invoke(color, target);
        }
    }
}