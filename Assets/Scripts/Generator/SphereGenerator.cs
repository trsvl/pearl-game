using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator
{
    public class SphereGenerator : MonoBehaviour
    {
        [Header("Sphere Properties")] public float smallSphereRadiusScale = 1.5f;
        public int smallSphereCount = 500;
        public float largeSphereRadius = 2f;

        [Header("Material Settings")] public Material[] materials;
        public float[] colorPercentages;

        [Header("Runtime Control")] [Range(1, 1000)]
        public int smallSphereCountRuntime = 500;

        [Range(0.1f, 10f)] public float smallSphereRadiusScaleRuntime = 1.5f;
        [Range(0.1f, 10f)] public float largeSphereRadiusRuntime = 2f;

        [Header("Chunk Control")] public int maxSpheresPerChunk = 20;

        private readonly List<GameObject> smallSpheres = new();
        private readonly List<Vector3> spherePositions = new();


        private void Start() => GenerateFibonacciSphere();

        private void Update()
        {
            if (smallSphereCount != smallSphereCountRuntime ||
                !Mathf.Approximately(smallSphereRadiusScale, smallSphereRadiusScaleRuntime) ||
                !Mathf.Approximately(largeSphereRadius, largeSphereRadiusRuntime))
            {
                smallSphereCount = smallSphereCountRuntime;
                smallSphereRadiusScale = smallSphereRadiusScaleRuntime;
                largeSphereRadius = largeSphereRadiusRuntime;
                ClearSpheres();
                GenerateFibonacciSphere();
            }
        }

        private void GenerateFibonacciSphere()
        {
            if (materials.Length != colorPercentages.Length)
            {
                Debug.LogError("Color percentages must match the number of materials.");
                return;
            }

            spherePositions.Clear();
            float smallSphereRadius = CalculateSmallSphereRadius(largeSphereRadius, smallSphereCount);

            for (int i = 0; i < smallSphereCount; i++)
            {
                float y = 1f - (i / (float)(smallSphereCount - 1)) * 2f;
                float radiusAtY = Mathf.Sqrt(1f - y * y);
                float theta = Mathf.PI * (3f - Mathf.Sqrt(5f)) * i;

                Vector3 position = new Vector3(
                    Mathf.Cos(theta) * radiusAtY,
                    y,
                    Mathf.Sin(theta) * radiusAtY
                ) * largeSphereRadius;

                spherePositions.Add(position);
            }

            // Assign materials to create chunks
            Material[] sphereMaterials = new Material[smallSphereCount];
            bool[] isAssigned = new bool[smallSphereCount];

            for (int matIndex = 0; matIndex < materials.Length; matIndex++)
            {
                Material currentMat = materials[matIndex];
                float percentage = colorPercentages[matIndex];
                int totalSpheres = Mathf.RoundToInt(smallSphereCount * percentage);

                int numChunks = Mathf.CeilToInt((float)totalSpheres / maxSpheresPerChunk);
                int remaining = totalSpheres;

                for (int chunk = 0; chunk < numChunks; chunk++)
                {
                    int chunkSize = Mathf.Min(maxSpheresPerChunk, remaining);
                    if (chunkSize <= 0) break;

                    // Find seed position
                    int seedIndex = -1;
                    for (int i = 0; i < smallSphereCount; i++)
                    {
                        if (!isAssigned[i])
                        {
                            seedIndex = i;
                            break;
                        }
                    }

                    if (seedIndex == -1) break;

                    sphereMaterials[seedIndex] = currentMat;
                    isAssigned[seedIndex] = true;
                    remaining--;
                    chunkSize--;

                    var availableIndices = Enumerable.Range(0, smallSphereCount)
                        .Where(i => !isAssigned[i])
                        .OrderBy(i => Vector3.Distance(spherePositions[i], spherePositions[seedIndex]))
                        .Take(chunkSize)
                        .ToList();

                    foreach (int idx in availableIndices)
                    {
                        sphereMaterials[idx] = currentMat;
                        isAssigned[idx] = true;
                        remaining--;
                    }
                }
            }

            for (int i = 0; i < smallSphereCount; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = spherePositions[i];
                sphere.transform.localScale = Vector3.one * (smallSphereRadius * 2f);
                sphere.transform.parent = transform;
                sphere.GetComponent<Renderer>().material = sphereMaterials[i] ?? materials[0];
                smallSpheres.Add(sphere);
            }
        }

        private float CalculateSmallSphereRadius(float largeRadius, int numSpheres)
        {
            float surfaceArea = 4f * Mathf.PI * largeRadius * largeRadius;
            float smallArea = surfaceArea / numSpheres;
            return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * smallSphereRadiusScale;
        }

        private void ClearSpheres()
        {
            foreach (GameObject sphere in smallSpheres)
                DestroyImmediate(sphere);
            smallSpheres.Clear();
            spherePositions.Clear();
        }
    }
}