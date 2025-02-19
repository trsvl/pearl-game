using UnityEngine;
using Utils.Colors;

namespace Utils.SphereData
{
    public class SphereGenerator : MonoBehaviour
    {
        public Vector3 _childQuaternion = new(0.19f, -0.9f, 0.17f);
        public bool _isStaticSize { get; private set; }
        public float _sphereLocalScaleRadius { get; private set; }
        public float _smallSphereRadiusScale { get; private set; }
        public Color[] _levelColors { get; private set; }

        private GameObject _prefabSphere;
        private AllColors _allColors;
        private AllSpheres _allSpheres;


        public void Init(GameObject prefabSphere, AllColors allColors, AllSpheres allSpheres)
        {
            _prefabSphere = prefabSphere;
            _allColors = allColors;
            _allSpheres = allSpheres;
        }

        private void Update()
        {
            transform.rotation *= Quaternion.Euler(12f * Time.deltaTime, 12f * Time.deltaTime, 0);

            foreach (Transform child in transform)
            {
                child.rotation = new Quaternion(_childQuaternion.x, _childQuaternion.y, _childQuaternion.z, 1f);
            }
        }

        public void LoadSpheresFromJSON(SpheresJSON json)
        {
            _smallSphereRadiusScale = json.smallSphereRadiusScale;
            _levelColors = new Color[json.colorNames.Length];
            _isStaticSize = json.isStaticRadius;
            _sphereLocalScaleRadius = json.smallSphereRadius;

            BigSphere[] bigSpheres = new BigSphere[json.spheres.Length];

            for (int i = 0; i < json.colorNames.Length; i++)
            {
                Color color = _allColors.GetColor(json.colorNames[i]);
                _allSpheres.AddBigSphereToDictionary(color, json.spheres.Length);
                _levelColors[i] = color;
            }

            for (int i = 0; i < json.spheres.Length; i++)
            {
                BigSphere newBigSphere = new BigSphere();
                newBigSphere.GenerateSmallSpherePositions(this, json.spheres[i], _levelColors, _allSpheres, i);
                bigSpheres[i] = newBigSphere;
            }
            
            var materialPropertyBlock = new MaterialPropertyBlock();

            foreach (var pair in _allSpheres.Get())
            {
                materialPropertyBlock.SetColor(AllColors.BaseColor, pair.Key);

                for (int i = 0; i < pair.Value.Length; i++)
                {
                    foreach (Vector3 localPosition in pair.Value[i])
                    {
                        GameObject sphere = Instantiate(_prefabSphere, transform);
                        bigSpheres[i].CreateSmallSphere(sphere, localPosition, materialPropertyBlock);
                    }
                }
            }
        }
    }
}