using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.SphereData
{
    [Serializable]
    public class Sphere
    {
        private int _smallSphereCount;
        private float _largeSphereRadius;
        private float smallSphereRadiusScale;
        private SphereGenerator _generator;
        private readonly List<GameObject> smallSpheres = new();
        private readonly List<Vector3> spherePositions = new();

        public void ClearSpheres()
        {
            foreach (GameObject sphere in smallSpheres)
                Object.DestroyImmediate(sphere);
            smallSpheres.Clear();
            spherePositions.Clear();
        }

        private void GeneratePositions()
        {
            float offset = 2f / _smallSphereCount;
            float increment = Mathf.PI * (3f - Mathf.Sqrt(5f));

            for (int i = 0; i < _smallSphereCount; i++)
            {
                float y = ((i + 0.5f) * offset) - 1f;
                float radiusAtY = Mathf.Sqrt(1f - y * y);
                float theta = increment * i;

                Vector3 position = new Vector3(
                    Mathf.Cos(theta) * radiusAtY,
                    y,
                    Mathf.Sin(theta) * radiusAtY
                ) * _largeSphereRadius;

                spherePositions.Add(position);
            }
        }

        private float CalculateSmallSphereRadius()
        {
            if (_generator._isStaticSize)
            {
                return _generator._sphereLocalScaleRadius;
            }

            float surfaceArea = 4f * Mathf.PI * _largeSphereRadius * _largeSphereRadius;
            float smallArea = surfaceArea / _smallSphereCount;
            return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * smallSphereRadiusScale;
        }

        public void GenerateFromJSON(SphereGenerator generator, SphereJSON json, Material[] materials)
        {
            _generator = generator;

            _smallSphereCount = json.smallSphereCount;
            _largeSphereRadius = json.largeSphereRadius;
            smallSphereRadiusScale = _generator._smallSphereRadiusScale;

            ClearSpheres();
            GeneratePositions();

            float smallSphereRadius = CalculateSmallSphereRadius();

            for (int i = 0; i < _smallSphereCount; i++)
            {
                GameObject sphere = Object.Instantiate(generator._prefabSphere, generator.transform);
                sphere.transform.localPosition = spherePositions[i];
                sphere.transform.localScale = Vector3.one * (smallSphereRadius * 2f);
                sphere.GetComponent<Renderer>().material = materials[json.materialIndexes[i]];
                smallSpheres.Add(sphere);
            }
        }
    }
}