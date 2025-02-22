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

        protected override void Update()
        {
            base.Update();

            foreach (var sphere in _bigSpheres)
            {
                foreach (var colorData in sphere.colorData)
                {
                    if (sphere._smallSphereCount == sphere.smallSphereCountRuntime &&
                        Mathf.Approximately(sphere._largeSphereRadius, sphere.largeSphereRadiusRuntime) &&
                        Mathf.Approximately(sphere._maxSpheresPerChunk, sphere.maxSpheresPerChunkRuntime) &&
                        Mathf.Approximately(sphere._smallSphereScale, sphere.smallSphereScaleRuntime) &&
                        Mathf.Approximately(colorData.colorPercentage, colorData.colorPercentageRuntime)) continue;

                    colorData.colorPercentage = colorData.colorPercentageRuntime;

                    print("Generating sphere");

                    GenerateNewSpheres();
                }
            }
        }

        public void GenerateNewSpheres()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < _bigSpheres.Length; i++)
            {
                _bigSpheres[i].GenerateSmallSpherePositions(this, i);
            }

            GenerateSpheres(_bigSpheres);
        }

        public override void LoadSpheresFromJSON(SpheresJSON json)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            print("load from json");

            _levelColors = new Color[json.colorNames.Length];
            var colorNames = new ColorName[json.colorNames.Length];
            _bigSpheres = new BigSphereBuilder[json.spheres.Length];

            for (int i = 0; i < json.colorNames.Length; i++)
            {
                Color color = _allColors.GetColor(json.colorNames[i]);
                _allSpheresData.AddColorToDictionary(color, json.spheres.Length);
                colorNames[i] = Enum.Parse<ColorName>(json.colorNames[i]);
                _levelColors[i] = color;
            }

            for (int i = 0; i < json.spheres.Length; i++)
            {
                BigSphereBuilder newBigSphere = new BigSphereBuilder();
                newBigSphere.GenerateSmallSpherePositions(json.spheres[i], _levelColors, colorNames, _allSpheresData,
                    i);
                _bigSpheres[i] = newBigSphere;
            }

            GenerateSpheres(_bigSpheres);
        }

        public (SphereGeneratorBuilder sphereGenerator, string[] colorNames, List<int[]> colorIds)
            GetDataForJSON()
        {
            HashSet<string> newColorNames = new HashSet<string>();
            List<int[]> newColorIds = new List<int[]>();

            foreach (BigSphereBuilder sphere in _bigSpheres)
            {
                foreach (var colorData in sphere.colorData)
                {
                    newColorNames.Add(colorData.ToString());
                }

                newColorIds.Add(sphere.colorIds);
            }

            return (this, newColorNames.ToArray(), newColorIds);
        }
    }
}