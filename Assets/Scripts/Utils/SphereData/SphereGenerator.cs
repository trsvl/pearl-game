using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Utils.Colors;
using Vector2 = System.Numerics.Vector2;

namespace Utils.SphereData
{
    public class SphereGenerator : MonoBehaviour
    {
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

            foreach (var pair in _allSpheres.Get())
            {
                var materialPropertyBlock = new MaterialPropertyBlock();
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