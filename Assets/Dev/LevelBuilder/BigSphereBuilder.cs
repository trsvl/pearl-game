using System;
using System.Linq;
using UnityEngine;
using Utils.SphereData;

namespace Dev.LevelBuilder
{
    [Serializable]
    public class ColorData
    {
        [Range(0.1f, 1f)] public float colorPercentage;
        public ColorName colorName;
    }

    [Serializable]
    public class BigSphereBuilder : BigSphere
    {
        [Range(1, 1000)] public int smallSphereCountRuntime = 500;
        [Range(0.1f, 10f)] public float largeSphereRadiusRuntime = 2f;
        [Range(10, 500)] public int maxSpheresPerChunkRuntime = 20;

        public int _maxSpheresPerChunk { get; set; }
        public int[] colorIds { get; private set; }

        public ColorData[] colorData;

        private Vector3[] localSpherePositions;


        public void GenerateSmallSpherePositions(SphereGeneratorBuilder generator, int bigSphereIndex)
        {
            _smallSphereCount = smallSphereCountRuntime;
            _largeSphereRadius = largeSphereRadiusRuntime;
            _smallSphereScale = generator._smallSphereScaleRuntime;
            _smallSphereRadius = generator._smallSphereRadius;
            
            _maxSpheresPerChunk = maxSpheresPerChunkRuntime;

            GeneratePositions();
            GenerateColors(generator, bigSphereIndex);
        }

        private void GeneratePositions()
        {
            localSpherePositions = new Vector3[_smallSphereCount];

            for (int i = 0; i < _smallSphereCount; i++)
            {
                localSpherePositions[i] = GeneratePosition(i);
            }
        }

        private void GenerateColors(SphereGeneratorBuilder generator, int bigSphereIndex)
        {
            ColorName[] sphereColorNames = colorData.Select(mc => mc.colorName).ToArray();
            Color[] sphereColors = colorData.Select(mc => generator._allColors.GetColor(mc.colorName.ToString()))
                .ToArray();
            float[] percentages = colorData.Select(mc => mc.colorPercentage).ToArray();

            bool[] isAssigned = new bool[_smallSphereCount];
            colorIds = new int[_smallSphereCount];
            generator._allSpheresData.Get().Clear();

            for (int colorIndex = 0; colorIndex < sphereColors.Length; colorIndex++)
            {
                float percentage = percentages[colorIndex];
                int totalSpheres = Mathf.RoundToInt(_smallSphereCount * percentage);

                int numChunks = Mathf.CeilToInt((float)totalSpheres / _maxSpheresPerChunk);
                int remaining = totalSpheres;

                generator._allSpheresData.AddColorToDictionary(sphereColors[colorIndex], generator._bigSpheres.Length);

                for (int chunk = 0; chunk < numChunks; chunk++)
                {
                    int chunkSize = Mathf.Min(_maxSpheresPerChunk, remaining);
                    if (chunkSize <= 0) break;

                    int seedIndex = FindFirstUnassigned(isAssigned);
                    if (seedIndex == -1) break;

                    colorIds[seedIndex] = colorIndex;
                    isAssigned[seedIndex] = true;
                    remaining--;
                    chunkSize--;

                    var availableIndices = Enumerable.Range(0, _smallSphereCount)
                        .Where(i => !isAssigned[i])
                        .OrderBy(i => Vector3.Distance(localSpherePositions[i], localSpherePositions[seedIndex]))
                        .Take(chunkSize)
                        .ToList();

                    foreach (int idx in availableIndices)
                    {
                        colorIds[idx] = colorIndex;
                        isAssigned[idx] = true;
                        remaining--;
                    }
                }
            }

            for (int i = 0; i < _smallSphereCount; i++)
            {
                Color color = generator._allColors.GetColor(sphereColorNames[colorIds[i]].ToString());
                generator._allSpheresData.AddSphere(color, localSpherePositions[i], bigSphereIndex);
            }
        }

        private int FindFirstUnassigned(bool[] assigned)
        {
            for (int i = 0; i < assigned.Length; i++)
                if (!assigned[i])
                    return i;
            return -1;
        }
    }
}