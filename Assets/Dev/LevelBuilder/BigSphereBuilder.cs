using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.SphereData;
using UnityEngine;

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

        public List<ColorData> sphereColorData;

        public int _maxSpheresPerChunk { get; set; }
        public float _smallSphereScale { get; set; }

        Dictionary<ColorName, int> colorCounts = new();
        private Vector3[] localSpherePositions;


        public BigSphereBuilder(BigSphereData data, string[] colorNames) : base(data)
        {
            _smallSphereCount = data.smallSphereCount;
            smallSphereCountRuntime = data.smallSphereCount;

            _largeSphereRadius = data.largeSphereRadius;
            largeSphereRadiusRuntime = data.largeSphereRadius;

            _maxSpheresPerChunk = 50;
            maxSpheresPerChunkRuntime = 50;

            _smallSphereScale = SPHERE_SCALE;
            smallSphereScaleRuntime = SPHERE_SCALE;

            GetSphereColors(data.colorIndexes, colorNames);

            CreateSphereColorData();
        }

        private void GetSphereColors(int[] colorIndexes, string[] colorNames)
        {
            foreach (int colorIndex in colorIndexes)
            {
                ColorName colorName = Enum.Parse<ColorName>(colorNames[colorIndex]);

                if (!colorCounts.TryAdd(colorName, 1)) colorCounts[colorName]++;
            }
        }

        private void CreateSphereColorData()
        {
            sphereColorData = new List<ColorData>();

            foreach (ColorName colorName in colorCounts.Keys)
            {
                var newColorData = new ColorData
                {
                    colorPercentage = colorCounts[colorName] / (float)_smallSphereCount,
                    colorPercentageRuntime = colorCounts[colorName] / (float)_smallSphereCount,
                    colorName = colorName,
                };

                sphereColorData.Add(newColorData);
            }
        }

        public BigSphereData GenerateBigSphereDataRuntime(string[] colorNames)
        {
            _smallSphereCount = smallSphereCountRuntime;
            _largeSphereRadius = largeSphereRadiusRuntime;
            _maxSpheresPerChunk = maxSpheresPerChunkRuntime;
            _smallSphereScale = smallSphereScaleRuntime;

            foreach (ColorData colorData in sphereColorData)
            {
                colorData.colorPercentage = colorData.colorPercentageRuntime;
            }
           
            GenerateLocalSpherePositions();

            int[] colorIndexes = GenerateColorIndexes(colorNames);

            BigSphereData bigSphereData = new BigSphereData()
            {
                smallSphereCount = _smallSphereCount,
                largeSphereRadius = _largeSphereRadius,
                colorIndexes = colorIndexes
            };

            return bigSphereData;
        }

        private int[] GenerateColorIndexes(string[] colorNames)
        {
            int[] colorIndexes = sphereColorData.Select(data => Array.IndexOf(colorNames, data.colorName.ToString()))
                .ToArray();

            float[] percentages = sphereColorData.Select(data => data.colorPercentage).ToArray();

            bool[] isAssigned = new bool[_smallSphereCount];
            var allIndexes = Enumerable.Repeat(-1, _smallSphereCount).ToArray();

            for (int colorIndex = 0; colorIndex < colorIndexes.Length; colorIndex++)
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

                    allIndexes[seedIndex] = colorIndexes[colorIndex];
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
                        allIndexes[idx] = colorIndexes[colorIndex];
                        isAssigned[idx] = true;
                        remaining--;
                    }
                }
            }

            int fallbackColor = colorIndexes.Length > 0 ? colorIndexes[0] : 0;
            for (int i = 0; i < _smallSphereCount; i++)
            {
                if (allIndexes[i] == -1)
                {
                    allIndexes[i] = fallbackColor;
                }
            }

            return allIndexes;
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