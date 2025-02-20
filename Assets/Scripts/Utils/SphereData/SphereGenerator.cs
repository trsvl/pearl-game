using UnityEngine;

namespace Utils.SphereData
{
    public class SphereGenerator : MonoBehaviour
    {
        public float _smallSphereRadius { get; protected set; }
        public float _smallSphereScale { get; protected set; }

        public Color[] _levelColors { get; private set; }
        public AllColors _allColors {get; private set;}
        public AllSpheresData _allSpheresData {get; private set;}
        public Vector3 _childQuaternion = new(0.19f, -0.9f, 0.17f);

        private GameObject _prefabSphere;
        private Quaternion _sphereRotation;


        public void Init(GameObject prefabSphere, AllColors allColors, AllSpheresData allSpheresData)
        {
            _prefabSphere = prefabSphere;
            _allColors = allColors;
            _allSpheresData = allSpheresData;
            _sphereRotation = new Quaternion(_childQuaternion.x, _childQuaternion.y, _childQuaternion.z, 1f);
        }

        protected virtual void Update()
        {
            transform.rotation *= Quaternion.Euler(12f * Time.deltaTime, 12f * Time.deltaTime, 0);

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = _sphereRotation;
            }

            //EDIT LIFETIME ROTATION

            // foreach (Transform child in transform)
            // {
            //     child.rotation = new Quaternion(_childQuaternion.x, _childQuaternion.y, _childQuaternion.z, 1f);
            // }
        }

        public virtual void LoadSpheresFromJSON(SpheresJSON json)
        {
            _smallSphereScale = json.smallSphereScale;
            _levelColors = new Color[json.colorNames.Length];
            _smallSphereRadius = json.smallSphereRadius;

            BigSphere[] bigSpheres = new BigSphere[json.spheres.Length];

            for (int i = 0; i < json.colorNames.Length; i++)
            {
                Color color = _allColors.GetColor(json.colorNames[i]);
                _allSpheresData.AddColorToDictionary(color, json.spheres.Length);
                _levelColors[i] = color;
            }

            FillSpheresData(json, bigSpheres);

            GenerateSpheres(bigSpheres);
        }

        private void FillSpheresData(SpheresJSON json, BigSphere[] bigSpheres)
        {
            for (int i = 0; i < json.spheres.Length; i++)
            {
                BigSphere newBigSphere = new BigSphere();
                newBigSphere.GenerateSmallSpherePositions(json.spheres[i], _levelColors, _allSpheresData,
                    _smallSphereScale, _smallSphereRadius, i);
                bigSpheres[i] = newBigSphere;
            }
        }

        protected void GenerateSpheres(BigSphere[] bigSpheres)
        {
            var materialPropertyBlock = new MaterialPropertyBlock();

            foreach (var pair in _allSpheresData.Get())
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