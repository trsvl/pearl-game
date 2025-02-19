using System;
using UnityEngine;

namespace Utils.SphereData
{
    [Serializable]
    public class BigSphere
    {
        private int _smallSphereCount;
        private float _largeSphereRadius;
        private float smallSphereRadiusScale;
        private SphereGenerator _generator;


        public void GenerateSmallSpherePositions(SphereGenerator generator, SphereJSON json, Color[] levelColors,
            AllSpheres allSpheres, int bigSphereIndex)
        {
            _generator = generator;

            _smallSphereCount = json.smallSphereCount;
            _largeSphereRadius = json.largeSphereRadius;
            smallSphereRadiusScale = _generator._smallSphereRadiusScale;

            for (int i = 0; i < _smallSphereCount; i++)
            {
                Color color = levelColors[json.colorIndexes[i]];
                allSpheres.AddSphere(color, GeneratePosition(i), bigSphereIndex);
            }
        }

        public void CreateSmallSphere(GameObject sphere, Vector3 localPosition,
            MaterialPropertyBlock materialPropertyBlock)
        {
            float smallSphereRadius = CalculateSmallSphereRadius();

            sphere.transform.localPosition = localPosition;
            sphere.transform.localScale = Vector3.one * (smallSphereRadius * 2.3f);
            sphere.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
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

        private Vector3 GeneratePosition(int index)
        {
            float offset = 2f / _smallSphereCount;
            float increment = Mathf.PI * (3f - Mathf.Sqrt(5f));

            float y = ((index + 0.5f) * offset) - 1f;
            float radiusAtY = Mathf.Sqrt(1f - y * y);
            float theta = increment * index;

            Vector3 position = new Vector3(
                Mathf.Cos(theta) * radiusAtY,
                y,
                Mathf.Sin(theta) * radiusAtY
            ) * _largeSphereRadius;

            return position;
        }
    }
}