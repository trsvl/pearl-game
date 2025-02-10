using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator
{
    [Serializable]
    public class MaterialController
    {
        [Range(0.1f, 1f)] public float colorPercentage;
        public Material material;
    }

    [Serializable]
    public class Sphere
    {
        [Header("Sphere Properties")]
        public int smallSphereCount = 500;
        public float largeSphereRadius = 2f;
        public float smallSphereRadiusScale = 2f;

        [Header("Runtime Control")]
        [Range(1, 1000)] public int smallSphereCountRuntime = 500;
        [Range(0.1f, 10f)] public float smallSphereRadiusScaleRuntime = 1.5f;
        [Range(0.1f, 10f)] public float largeSphereRadiusRuntime = 2f;
        [Range(10, 500)] public int maxSpheresPerChunk = 20;

        [Header("Material Settings")]
        public MaterialController[] materialControllers;

        [NonSerialized] private readonly List<GameObject> smallSpheres = new();
        [NonSerialized] private readonly List<Vector3> spherePositions = new();

        public void ClearSpheres()
        {
            foreach (GameObject sphere in smallSpheres)
                UnityEngine.Object.DestroyImmediate(sphere);
            smallSpheres.Clear();
            spherePositions.Clear();
        }

        public void Generate(GameObject prefab, Transform parent)
        {
            // Validate materials
            if (materialControllers == null || materialControllers.Length == 0)
            {
                Debug.LogError("No material controllers defined!");
                return;
            }

            // Update runtime values
            smallSphereCount = smallSphereCountRuntime;
            smallSphereRadiusScale = smallSphereRadiusScaleRuntime;
            largeSphereRadius = largeSphereRadiusRuntime;

            ClearSpheres();
            GeneratePositions();
            AssignMaterials(prefab, parent);
        }

        private void GeneratePositions()
        {
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
        }

        private void AssignMaterials(GameObject prefab, Transform parent)
        {
            Material[] materials = materialControllers.Select(mc => mc.material).ToArray();
            float[] percentages = materialControllers.Select(mc => mc.colorPercentage).ToArray();
            
            Material[] sphereMaterials = new Material[smallSphereCount];
            bool[] isAssigned = new bool[smallSphereCount];
            float smallSphereRadius = CalculateSmallSphereRadius();

            for (int matIndex = 0; matIndex < materials.Length; matIndex++)
            {
                Material currentMat = materials[matIndex];
                float percentage = percentages[matIndex];
                int totalSpheres = Mathf.RoundToInt(smallSphereCount * percentage);

                int numChunks = Mathf.CeilToInt((float)totalSpheres / maxSpheresPerChunk);
                int remaining = totalSpheres;

                for (int chunk = 0; chunk < numChunks; chunk++)
                {
                    int chunkSize = Mathf.Min(maxSpheresPerChunk, remaining);
                    if (chunkSize <= 0) break;

                    int seedIndex = FindFirstUnassigned(isAssigned);
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

            // Create spheres
            for (int i = 0; i < smallSphereCount; i++)
            {
                GameObject sphere = UnityEngine.Object.Instantiate(prefab, parent);
                sphere.transform.localPosition = spherePositions[i];
                sphere.transform.localScale = Vector3.one * (smallSphereRadius * 2f);
                sphere.GetComponent<Renderer>().material = sphereMaterials[i] ?? materials[0];
                smallSpheres.Add(sphere);
            }
        }

        private float CalculateSmallSphereRadius()
        {
            float surfaceArea = 4f * Mathf.PI * largeSphereRadius * largeSphereRadius;
            float smallArea = surfaceArea / smallSphereCount;
            return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * smallSphereRadiusScale;
        }

        private int FindFirstUnassigned(bool[] assigned)
        {
            for (int i = 0; i < assigned.Length; i++)
                if (!assigned[i]) return i;
            return -1;
        }
    }

    public class SphereGenerator : MonoBehaviour
    {
        public GameObject prefabSphere;
        public Sphere[] spheres;
        
        private Transform parent;

        private void Start()
        {
            GameObject spheresParent = new GameObject("Spheres");
            parent = spheresParent.transform;
            GenerateAllSpheres(parent);
        }

        private void Update()
        {
            foreach (var sphere in spheres)
            {
                if (sphere.smallSphereCount != sphere.smallSphereCountRuntime ||
                    !Mathf.Approximately(sphere.smallSphereRadiusScale, sphere.smallSphereRadiusScaleRuntime) ||
                    !Mathf.Approximately(sphere.largeSphereRadius, sphere.largeSphereRadiusRuntime))
                {
                    GenerateAllSpheres(parent);
                    break;
                }
            }
        }

        private void GenerateAllSpheres(Transform parentObj)
        {
            foreach (Transform child in parentObj)
            {
                Destroy(child.gameObject);
            }

            foreach (var sphere in spheres)
            {
                sphere.Generate(prefabSphere, parentObj);
            }
        }
    }
}