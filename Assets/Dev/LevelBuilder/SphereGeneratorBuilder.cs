using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.SphereData;

namespace Dev.LevelBuilder
{
    public class SphereGeneratorBuilder : SphereGenerator
    {
        public BigSphereBuilder[] _bigSpheres;

        private SpheresData _spheresData;


        protected override void Update()
        {
            base.Update();

            foreach (Transform child in transform)
            {
                child.rotation = new Quaternion(_childQuaternion.x, _childQuaternion.y, _childQuaternion.z, 1f);
            }

            foreach (var sphere in _bigSpheres)
            {
                foreach (var colorData in sphere.colorData) //!!!
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

            for (int i = 0; i < _bigSpheres.Length; i++)
            {
                BigSphereData sphereData = _bigSpheres[i].GenerateBigSphereDataRuntime(_allColors);
                spheresData.spheres[i] = sphereData;

                foreach (var colorData in _bigSpheres[i].colorData)
                {
                    colorNames.Add(colorData.colorName.ToString());
                }
            }

            spheresData.colorNames = colorNames.ToArray();

            _spheresData = spheresData;
            return spheresData;
        }

        protected override void GenerateBigSphereData(SpheresData data)
        {
            var colorNames = new ColorName[data.colorNames.Length];

            for (int i = 0; i < data.colorNames.Length; i++)
            {
                colorNames[i] = Enum.Parse<ColorName>(data.colorNames[i]);
            }

            _bigSpheres = new BigSphereBuilder[data.spheres.Length];

            for (int i = 0; i < _bigSpheres.Length; i++)
            {
                var bigSphere = new BigSphereBuilder(data.spheres[i], colorNames);
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