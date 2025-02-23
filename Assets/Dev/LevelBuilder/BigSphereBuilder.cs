using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.SphereData;

namespace Dev.LevelBuilder
{
    [Serializable]
    public class ColorData
    {
        public float colorPercentage { get; set; }
        [Range(0.1f, 1f)] public float colorPercentageRuntime;
        public ColorName colorName;
    }

    [Serializable]
    public class BigSphereBuilder : BigSphere
    {
        [Range(1, 1000)] public int smallSphereCountRuntime;
        [Range(0.1f, 10f)] public float largeSphereRadiusRuntime;
        [Range(10, 500)] public int maxSpheresPerChunkRuntime;
        [Space] [Range(0f, 3f)] public float smallSphereScaleRuntime;

        public List<ColorData> colorData;

        public int _maxSpheresPerChunk { get; set; }
        public float _smallSphereScale { get; set; }

        private Vector3[] localSpherePositions;


        public BigSphereBuilder(BigSphereData data, ColorName[] colorNames) : base(data)
        {
            _smallSphereCount = data.smallSphereCount;
            smallSphereCountRuntime = data.smallSphereCount;

            _largeSphereRadius = data.largeSphereRadius;
            largeSphereRadiusRuntime = data.largeSphereRadius;

            _maxSpheresPerChunk = 50;
            maxSpheresPerChunkRuntime = 50;

            _smallSphereScale = SPHERE_SCALE;
            smallSphereScaleRuntime = SPHERE_SCALE;

            HashSet<ColorName> sphereColorNames = new HashSet<ColorName>();
            Dictionary<ColorName, int> colorCounts = new Dictionary<ColorName, int>();


            foreach (int colorIndex in data.colorIndexes)
            {
                ColorName colorName = colorNames[colorIndex];
                sphereColorNames.Add(colorName);

                if (!colorCounts.ContainsKey(colorName)) colorCounts.Add(colorName, colorIndex + 1);
                else colorCounts[colorName]++;
            }

            colorData = new List<ColorData>(sphereColorNames.Count);

            foreach (ColorName colorName in sphereColorNames)
            {
                var newColorData = new ColorData()
                {
                    colorPercentage = colorCounts[colorName] / (float)_smallSphereCount,
                    colorPercentageRuntime = colorCounts[colorName] / (float)_smallSphereCount,
                    colorName = colorName,
                };

                colorData.Add(newColorData);
            }
        }

        public BigSphereData GenerateBigSphereDataRuntime(AllColors allColors)
        {
            _smallSphereCount = smallSphereCountRuntime;
            _largeSphereRadius = largeSphereRadiusRuntime;
            _maxSpheresPerChunk = maxSpheresPerChunkRuntime;
            _smallSphereScale = smallSphereScaleRuntime;

            GenerateLocalSpherePositions();
            int[] colorIndexes = GenerateColorIndexes(allColors);

            BigSphereData bigSphereData = new BigSphereData()
            {
                smallSphereCount = _smallSphereCount,
                largeSphereRadius = _largeSphereRadius,
                colorIndexes = colorIndexes
            };
//            ColorName[] sphereColorNames = colorData.Select(mc => mc.colorName).ToArray();

//            Color color = generator._allColors.GetColor(sphereColorNames[colorIndexes[i]].ToString());

            return bigSphereData;
        }

        private int[] GenerateColorIndexes(AllColors allColors)
        {
            Color[] sphereColors = colorData.Select(mc => allColors.GetColor(mc.colorName.ToString()))
                .ToArray();
            float[] percentages = colorData.Select(mc => mc.colorPercentage).ToArray();

            bool[] isAssigned = new bool[_smallSphereCount];
            var colorIndexes = new int[_smallSphereCount];

            for (int colorIndex = 0; colorIndex < sphereColors.Length; colorIndex++)
            {
                float percentage = percentages[colorIndex];
                int totalSpheres = Mathf.RoundToInt(_smallSphereCount * percentage);

                int numChunks = Mathf.CeilToInt((float)totalSpheres / _maxSpheresPerChunk);
                int remaining = totalSpheres;

                for (int chunk = 0; chunk < numChunks; chunk++)
                {
                    int chunkSize = Mathf.Min(_maxSpheresPerChunk, remaining);
                    if (chunkSize <= 0) break;

                    int seedIndex = FindFirstUnassigned(isAssigned);
                    if (seedIndex == -1) break;

                    colorIndexes[seedIndex] = colorIndex;
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
                        colorIndexes[idx] = colorIndex;
                        isAssigned[idx] = true;
                        remaining--;
                    }
                }
            }

            return colorIndexes;
        }

        private void GenerateLocalSpherePositions()
        {
            localSpherePositions = new Vector3[_smallSphereCount];

            for (int i = 0; i < _smallSphereCount; i++)
            {
                localSpherePositions[i] = GeneratePosition(i);
            }
        }

        private int FindFirstUnassigned(bool[] assigned)
        {
            for (int i = 0; i < assigned.Length; i++)
                if (!assigned[i])
                    return i;
            return -1;
        }

        protected override Vector3 GetLocalScale(GameObject sphere, float additionalScale)
        {
            float scale = smallSphereScaleRuntime == 0f ? SPHERE_SCALE : smallSphereScaleRuntime;
            return base.GetLocalScale(sphere, scale);
        }
    }
}