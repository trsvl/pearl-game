using UnityEngine;

namespace Utils.SphereData
{
    public class BigSphere
    {
        public int _smallSphereCount { get; protected set; }
        public float _largeSphereRadius { get; protected set; }
        protected const float SPHERE_SCALE = 2.3f;


        public void CreateSmallSpheres(GameObject sphere, SphereJSON json, Color[] levelColors,
            AllSpheresData allSpheresData, int bigSphereIndex)
        {
            _smallSphereCount = json.smallSphereCount;
            _largeSphereRadius = json.largeSphereRadius;
            
            var materialPropertyBlock = new MaterialPropertyBlock();

            for (int i = 0; i < _smallSphereCount; i++)
            {
                sphere.transform.localPosition = GeneratePosition(i);
                sphere.transform.localScale = GetLocalScale(sphere, SPHERE_SCALE);
                sphere.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
                
                Color color = levelColors[json.colorIndexes[i]];
                materialPropertyBlock.SetColor(AllColors.BaseColor, color);
                allSpheresData.AddSphere(color, sphere, bigSphereIndex);
            }
        }

        private float CalculateSmallSphereRadius()
        {
            float surfaceArea = 4f * Mathf.PI * _largeSphereRadius * _largeSphereRadius;
            float smallArea = surfaceArea / _smallSphereCount;
            const float smallRadiusScale = 2f;
            return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * smallRadiusScale;
        }

        protected virtual Vector3 GetLocalScale(GameObject sphere, float additionalScale)
        {
            float smallSphereRadius = CalculateSmallSphereRadius();

            return sphere.transform.localScale = Vector3.one * (smallSphereRadius * additionalScale);
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