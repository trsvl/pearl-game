using System.Linq;
using Gameplay.SphereData;
using UnityEngine;

namespace Gameplay.Animations
{
    public class CameraManager
    {
        private readonly Camera _mainCamera;
        private readonly Camera _uiCamera; //!!!
        private float _initialFOV;
        private int alreadyTriggeredIndex;


        public CameraManager(Camera uiCamera)
        {
            _mainCamera = Camera.main;
            _initialFOV = _mainCamera.fieldOfView;
            _uiCamera = uiCamera;
        }

        public float GetInitialFOV()
        {
            return _initialFOV;
        }

        public void AssignNewFOV()
        {
            _initialFOV = _mainCamera.fieldOfView;
        }

        public float CalculateNewFOV(int destroyedSphereLayers)
        {
            const float step = 6f;
            float newFOV = _initialFOV - destroyedSphereLayers * step;

            return newFOV;
        }

        public (Vector3 ballPosition, Vector3 nextBallPosition) UpdateBallsPositionAndFOV(float _ballSize,
            float newFOV = 0f)
        {
            _mainCamera.fieldOfView = newFOV == 0f ? _initialFOV : newFOV;

            float cameraView =
                2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _mainCamera.fieldOfView);
            float distance = 2f * _ballSize / cameraView;
            distance += 0.5f * _ballSize;
            _mainCamera.transform.position = new Vector3(0, 0, distance);

            Vector3 ballPosition = new Vector3(0.8f, 0.2f, distance);
            Vector3 nextBallPosition = new Vector3(0.6f, 0.1f, distance);
            var ballSpawnPoint = _mainCamera.ViewportToWorldPoint(ballPosition);
            var nextBallSpawnPoint = _mainCamera.ViewportToWorldPoint(nextBallPosition);

            return (ballSpawnPoint, nextBallSpawnPoint);
        }

        public Camera GetMainCamera()
        {
            return _mainCamera;
        }
    }
}