using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator
{
    public class SphereGenerator : MonoBehaviour
    {
        public bool _isActiveEdit;

        [Space] public GameObject _prefabSphere;
        public float _smallSphereRadiusScaleRuntime;
        public Sphere[] _spheres;

        [Space] public bool _isStaticSize = true;
        public float _sphereLocalScaleRadius;
        public Material[] _materials { get; set; }

        private float _smallSphereRadiusScale;


        private void Update()
        {
            transform.rotation *= Quaternion.Euler(12f * Time.deltaTime, 12f * Time.deltaTime, 0);

            if (!_isActiveEdit) return;

            foreach (var sphere in _spheres)
            {
                if (sphere.smallSphereCount != sphere.smallSphereCountRuntime ||
                    !Mathf.Approximately(_smallSphereRadiusScale, _smallSphereRadiusScaleRuntime) ||
                    !Mathf.Approximately(sphere.largeSphereRadius, sphere.largeSphereRadiusRuntime))
                {
                    sphere.smallSphereCount = sphere.smallSphereCountRuntime;
                    _smallSphereRadiusScale = _smallSphereRadiusScaleRuntime;
                    sphere.largeSphereRadius = sphere.largeSphereRadiusRuntime;

                    GenerateAllSpheres();
                    break;
                }
            }
        }

        private void GenerateAllSpheres()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var sphere in _spheres)
            {
                sphere.Generate(this);
            }
        }

        public void GenerateNewSpheres()
        {
            GenerateAllSpheres();
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

        public
            (
            SphereGenerator sphereGenerator,
            string[] materialNames,
            List<int[]>
            )
            GetDataForJSON()
        {
            HashSet<string> uniqueMaterialNames = new HashSet<string>();

            foreach (Sphere sphere in _spheres)
            {
                foreach (var controller in sphere.materialControllers)
                {
                    uniqueMaterialNames.Add(controller.material.name);
                }
            }

            string[] materialNames = uniqueMaterialNames.ToArray();

            List<int[]> materialIndexesList = new();

            foreach (Sphere sphere in _spheres)
            {
                int[] materialIndexes = new int[sphere.smallSphereCount];

                for (int i = 0; i < sphere.materialNames.Length; i++)
                {
                    materialIndexes[i] = Array.IndexOf(materialNames, sphere.materialNames[i]);
                }

                materialIndexesList.Add(materialIndexes);
            }

            return (this, materialNames, materialIndexesList);
        }
    }
}