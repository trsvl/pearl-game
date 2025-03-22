using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SphereData;
using UnityEngine;

namespace Dev.LevelBuilder
{
    public class SphereGeneratorBuilder : SphereGenerator
    {
        public BigSphereBuilder[] _bigSpheres;

        private SpheresData _spheresData;
        private bool isRuntimeChanges;


        protected override void Update()
        {
            base.Update();

            foreach (Transform child in transform)
            {
                child.rotation = SphereRotation.GetQuaternion;
            }

            foreach (var sphere in _bigSpheres)
            {
                foreach (var colorData in sphere.sphereColorData)
                {
                    if (sphere._smallSphereCount == sphere.smallSphereCountRuntime &&
                        Mathf.Approximately(sphere._largeSphereRadius, sphere.largeSphereRadiusRuntime) &&
                        Mathf.Approximately(sphere._maxSpheresPerChunk, sphere.maxSpheresPerChunkRuntime) &&
                        Mathf.Approximately(sphere._smallSphereScale, sphere.smallSphereScaleRuntime) &&
                        Mathf.Approximately(colorData.colorPercentage, colorData.colorPercentageRuntime)) continue;

                    isRuntimeChanges = true;
                    var data = GenerateSpheresData(true);
                    LoadSpheres(data);
                }
            }
        }

        private SpheresData GenerateSpheresData(bool destroySpheres)
        {
            if (destroySpheres)
            {
                ClearSpheres();
            }

            SpheresData spheresData = new SpheresData();

            spheresData.spheres = new BigSphereData[_bigSpheres.Length];

            HashSet<string> colorNames = new();

            foreach (BigSphereBuilder bigSphere in _bigSpheres)
            {
                foreach (var colorData in bigSphere.sphereColorData)
                {
                    colorNames.Add(colorData.colorName.ToString());
                }
            }

            for (int i = 0; i < _bigSpheres.Length; i++)
            {
                BigSphereData bigSphereData = _bigSpheres[i].GenerateBigSphereDataRuntime(colorNames.ToArray());
                spheresData.spheres[i] = bigSphereData;
            }

            spheresData.colorNames = colorNames.ToArray();

            _spheresData = spheresData;
            return spheresData;
        }

        protected override void GenerateBigSphereData(SpheresData data)
        {
            if (!isRuntimeChanges)
            {
                _bigSpheres = new BigSphereBuilder[data.spheres.Length];

                for (int i = 0; i < _bigSpheres.Length; i++)
                {
                    var bigSphere = new BigSphereBuilder(data.spheres[i], data.colorNames);

                    _bigSpheres[i] = bigSphere;

                    GenerateSmallSpheres(data, bigSphere, i);
                }
            }
            else
            {
                isRuntimeChanges = false;

                for (int i = 0; i < _bigSpheres.Length; i++)
                {
                    GenerateSmallSpheres(data, _bigSpheres[i], i);
                }
            }
        }

        public SpheresData GetSpheresData(bool destroySpheres)
        {
            return _spheresData ?? GenerateSpheresData(destroySpheres);
        }
    }
}