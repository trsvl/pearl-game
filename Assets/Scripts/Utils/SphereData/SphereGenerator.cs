using UnityEngine;

namespace Utils.SphereData
{
    public class SphereGenerator : MonoBehaviour
    {
        public GameObject _prefabSphere { get; private set; }
        public bool _isStaticSize { get; private set; }
        public float _sphereLocalScaleRadius { get; private set; }
        public float _smallSphereRadiusScale { get; private set; }
        public Material[] _materials { get; private set; }


        public void Init(GameObject prefabSphere)
        {
            _prefabSphere = prefabSphere;
        }

        private void Update()
        {
            transform.rotation *= Quaternion.Euler(12f * Time.deltaTime, 12f * Time.deltaTime, 0);
        }

        public void LoadSpheresFromJSON(SpheresJSON json)
        {
            _smallSphereRadiusScale = json.smallSphereRadiusScale;
            _materials = new Material[json.materialNames.Length];
            _isStaticSize = json.isStaticRadius;
            _sphereLocalScaleRadius = json.smallSphereRadius;

            for (int i = 0; i < json.materialNames.Length; i++)
            {
                var material = Resources.Load<Material>($"Materials/{json.materialNames[i]}");
                _materials[i] = material;
            }

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var sphereJson in json.spheres)
            {
                Sphere newSphere = new Sphere();
                newSphere.GenerateFromJSON(this, sphereJson, _materials);
            }
        }
    }
}