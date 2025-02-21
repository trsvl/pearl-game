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
                if (sphere._smallSphereCount == sphere.smallSphereCountRuntime &&
                    Mathf.Approximately(sphere._largeSphereRadius, sphere.largeSphereRadiusRuntime) &&
                    Mathf.Approximately(sphere._maxSpheresPerChunk, sphere.maxSpheresPerChunkRuntime)) continue;

                print("Generating sphere");

                GenerateNewSpheres();
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