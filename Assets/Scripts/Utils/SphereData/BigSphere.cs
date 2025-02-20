using UnityEngine;

namespace Utils.SphereData
{
    public class BigSphere
    {
        public int _smallSphereCount {get; set;}
        public float _largeSphereRadius {get; set;}
        protected float _smallSphereScale;
        protected float _smallSphereRadius;


        public void GenerateSmallSpherePositions(SphereJSON json, Color[] levelColors,
            AllSpheresData allSpheresData, float smallSphereRadiusScale, float smallSphereRadius, int bigSphereIndex)
        {
            _smallSphereCount = json.smallSphereCount;
            _largeSphereRadius = json.largeSphereRadius;
            _smallSphereScale = smallSphereRadiusScale;
            _smallSphereRadius = smallSphereRadius;

            for (int i = 0; i < _smallSphereCount; i++)
            {
                Color color = levelColors[json.colorIndexes[i]];
                allSpheresData.AddSphere(color, GeneratePosition(i), bigSphereIndex);
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

        protected float CalculateSmallSphereRadius()
        {
            if (!Mathf.Approximately(_smallSphereRadius, default))
            {
                return _smallSphereRadius;
            }

            float surfaceArea = 4f * Mathf.PI * _largeSphereRadius * _largeSphereRadius;
            float smallArea = surfaceArea / _smallSphereCount;
            return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * _smallSphereScale;
        }

        protected Vector3 GeneratePosition(int index)
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