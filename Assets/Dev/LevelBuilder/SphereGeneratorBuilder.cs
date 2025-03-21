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
        private List<ColorName>[] _sphereColors;


        protected override void Update()
        {
            base.Update();

            foreach (Transform child in transform)
            {
                child.rotation = SphereRotation.GetQuaternion;
            }

            foreach (var sphere in _bigSpheres)
            {
                foreach (var colorData in sphere.sphereData)
                {
                    if (sphere._smallSphereCount == sphere.smallSphereCountRuntime &&
                        Mathf.Approximately(sphere._largeSphereRadius, sphere.largeSphereRadiusRuntime) &&
                        Mathf.Approximately(sphere._maxSpheresPerChunk, sphere.maxSpheresPerChunkRuntime) &&
                        Mathf.Approximately(sphere._smallSphereScale, sphere.smallSphereScaleRuntime) &&
                        Mathf.Approximately(colorData.colorPercentage, colorData.colorPercentageRuntime)) continue;

                    colorData.colorPercentage = colorData.colorPercentageRuntime;

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

            _sphereColors = new List<ColorName>[_bigSpheres.Length];

            for (int i = 0; i < _bigSpheres.Length; i++)
            {
                BigSphereData sphereData = _bigSpheres[i].GenerateBigSphereDataRuntime(_allColors);
                spheresData.spheres[i] = sphereData;

                _sphereColors[i] = new List<ColorName>();

                foreach (var colorData in _bigSpheres[i].sphereData)
                {
                    colorNames.Add(colorData.colorName.ToString());
                    _sphereColors[i].Add(colorData.colorName);
                }
            }

            spheresData.colorNames = colorNames.ToArray();

            _spheresData = spheresData;
            return spheresData;
        }

        protected override void GenerateBigSphereData(SpheresData data)
        {
            _bigSpheres = new BigSphereBuilder[data.spheres.Length];

            for (int i = 0; i < _bigSpheres.Length; i++)
            {
                var bigSphere = new BigSphereBuilder(data.spheres[i], data.colorNames);

                _bigSpheres[i] = bigSphere;

                GenerateSmallSpheres(data, bigSphere, i);
            }
        }

        public SpheresData GetSpheresData(bool destroySpheres)
        {
            return _spheresData ?? GenerateSpheresData(destroySpheres);
        }
    }
}