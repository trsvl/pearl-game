using UnityEngine;

namespace Utils.SphereData
{
    public class BigSphere
    {
        public int _smallSphereCount { get; protected set; }
        public float _largeSphereRadius { get; protected set; }

        protected const float SPHERE_SCALE = 2.3f;


        public BigSphere(BigSphereData data)
        {
            _smallSphereCount = data.smallSphereCount;
            _largeSphereRadius = data.largeSphereRadius;
        }

        public void CreateSmallSpheres(GameObject spherePrefab, Transform parent, BigSphereData data,
            Color[] levelColors, AllSpheresData allSpheresData, int bigSphereIndex)
        {
            var materialPropertyBlock = new MaterialPropertyBlock();

            for (int i = 0; i < _smallSphereCount; i++)
            {
                GameObject sphere = Object.Instantiate(spherePrefab, parent);
                sphere.transform.localPosition = GeneratePosition(i);
                sphere.transform.localScale = GetLocalScale(sphere, SPHERE_SCALE);

                Color color = levelColors[data.colorIndexes[i]];
                materialPropertyBlock.SetColor(AllColors.BaseColor, color);
                allSpheresData.AddSphere(color, sphere, bigSphereIndex);

                sphere.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
            }
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

        protected virtual Vector3 GetLocalScale(GameObject sphere, float additionalScale)
        {
            float smallSphereRadius = CalculateSmallSphereRadius();

            return sphere.transform.localScale = Vector3.one * (smallSphereRadius * additionalScale);
        }
    }
}