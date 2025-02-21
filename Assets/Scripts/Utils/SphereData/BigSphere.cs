using UnityEngine;

namespace Utils.SphereData
{
    public class BigSphere
    {
        public int _smallSphereCount {get; set;}
        public float _largeSphereRadius {get; set;}


        public void GenerateSmallSpherePositions(SphereJSON json, Color[] levelColors,
            AllSpheresData allSpheresData, int bigSphereIndex)
        {
            _smallSphereCount = json.smallSphereCount;
            _largeSphereRadius = json.largeSphereRadius;

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

        private float CalculateSmallSphereRadius()
        {
            float surfaceArea = 4f * Mathf.PI * _largeSphereRadius * _largeSphereRadius;
            float smallArea = surfaceArea / _smallSphereCount;
            const float smallRadiusScale = 2f;
            return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * smallRadiusScale;
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