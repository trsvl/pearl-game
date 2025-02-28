using System.Linq;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay.BallThrowing
{
    public class CameraManager
    {
        private SpheresDictionary _spheresDictionary;
        private readonly Camera _mainCamera;
        private float _initialFOV;
        private int alreadyTriggeredIndex;


        public CameraManager(SpheresDictionary spheresDictionary)
        {
            _mainCamera = Camera.main;
            _initialFOV = _mainCamera.fieldOfView;
            _spheresDictionary = spheresDictionary;
        }

        public float GetInitialFOV()
        {
            return _initialFOV;
        }
       
        public void AssignNewFOV()
        {
            _initialFOV = _mainCamera.fieldOfView;
        }

        public float GetNewFOV()
        {
            var allSpheres = _spheresDictionary.GetSpheres();

            int length = allSpheres.First().Length;

            int destroyLayerCount = 0;

            for (int i = length - 1; i >= 0; i--)
            {
                bool isSphereLayerDestroyed = true;

                foreach (var sphereListArray in allSpheres)
                {
                    if (sphereListArray[i].Count == 0 && alreadyTriggeredIndex > i) continue;

                    isSphereLayerDestroyed = false;
                    break;
                }

                if (isSphereLayerDestroyed) destroyLayerCount++;
                else break;
            }

            return CalculateNewFOV(destroyLayerCount);
        }

        private float CalculateNewFOV(int destroyLayerIndex)
        {
            const float step = 6f;
            float newFOV = _initialFOV - destroyLayerIndex * step;

            return newFOV;
        }

        public Vector3 UpdateFOV(float _ballSize, float newFOV)
        {
            _mainCamera.fieldOfView = newFOV;

            float cameraView =
                2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _mainCamera.fieldOfView);
            float distance = 2f * _ballSize / cameraView;
            distance += 0.5f * _ballSize;
            _mainCamera.transform.position = new Vector3(0, 0, distance);

            Vector3 ballPosition = new Vector3(0.8f, 0.15f, distance);
            var ballSpawnPoint = _mainCamera.ViewportToWorldPoint(ballPosition);
            return ballSpawnPoint;
        }
    }
}