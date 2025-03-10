﻿using System.Linq;
using Gameplay.SphereData;
using UnityEngine;

namespace Gameplay.Animations
{
    public class CameraManager
    {
        private readonly SpheresDictionary _spheresDictionary;
        private readonly Camera _mainCamera;
        private readonly Camera _uiCamera; //!!!
        private float _initialFOV;
        private int alreadyTriggeredIndex;


        public CameraManager(SpheresDictionary spheresDictionary, Camera uiCamera)
        {
            _mainCamera = Camera.main;
            _initialFOV = _mainCamera.fieldOfView;
            _spheresDictionary = spheresDictionary;
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

        public (Vector3 ballPosition, Vector3 nextBallPosition) UpdateBallsPositionAndFOV(float _ballSize,
            float newFOV = 0f)
        {
            _mainCamera.fieldOfView = newFOV == 0f ? _initialFOV : newFOV;

            float cameraView =
                2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _mainCamera.fieldOfView);
            float distance = 2f * _ballSize / cameraView;
            distance += 0.5f * _ballSize;
            _mainCamera.transform.position = new Vector3(0, 0, distance);

            Vector3 ballPosition = new Vector3(0.8f, 0.15f, distance);
            Vector3 nextBallPosition = new Vector3(0.65f, 0.05f, distance);
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