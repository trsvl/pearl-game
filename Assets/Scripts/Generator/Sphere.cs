using System;
using System.Collections.Generic;
using System.Linq;
using Generator;
using UnityEngine;

[Serializable]
public class MaterialController
{
    [Range(0.1f, 1f)] public float colorPercentage;
    public Material material;
}

[Serializable]
public class Sphere
{
    public int smallSphereCount { get; set; } = 500;
    public float largeSphereRadius { get; set; } = 2f;
    public string[] materialNames { get; set; }

    [Range(1, 1000)] public int smallSphereCountRuntime = 500;
    [Range(0.1f, 10f)] public float largeSphereRadiusRuntime = 2f;
    [Range(10, 500)] public int maxSpheresPerChunk = 20;


    public MaterialController[] materialControllers;

    private float smallSphereRadiusScale;
    private SphereGenerator _generator;
    [NonSerialized] private readonly List<GameObject> smallSpheres = new();
    [NonSerialized] private readonly List<Vector3> spherePositions = new();

    public void ClearSpheres()
    {
        foreach (GameObject sphere in smallSpheres)
            UnityEngine.Object.DestroyImmediate(sphere);
        smallSpheres.Clear();
        spherePositions.Clear();
    }

    public void Generate(SphereGenerator generator)
    {
        if (materialControllers == null || materialControllers.Length == 0)
        {
            Debug.LogError("No material controllers defined!");
            return;
        }

        _generator = generator;

        smallSphereCount = smallSphereCountRuntime;
        largeSphereRadius = largeSphereRadiusRuntime;
        smallSphereRadiusScale = _generator._smallSphereRadiusScaleRuntime;

        ClearSpheres();
        GeneratePositions();
        AssignMaterials(_generator._prefabSphere, _generator.transform);
    }

    private void GeneratePositions()
    {
        float offset = 2f / smallSphereCount;
        float increment = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < smallSphereCount; i++)
        {
            float y = ((i + 0.5f) * offset) - 1f;
            float radiusAtY = Mathf.Sqrt(1f - y * y);
            float theta = increment * i;

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

        materialNames = new string[smallSphereCount];

        for (int i = 0; i < smallSphereCount; i++)
        {
            GameObject sphere = UnityEngine.Object.Instantiate(prefab, parent);
            sphere.transform.localPosition = spherePositions[i];
            sphere.transform.localScale = Vector3.one * (smallSphereRadius * 2f);
            var material = sphere.GetComponent<Renderer>().material = sphereMaterials[i] ?? materials[0];
            materialNames[i] = material.name;
            smallSpheres.Add(sphere);
        }
    }

    private float CalculateSmallSphereRadius()
    {
        if (_generator._isStaticSize)
        {
            return _generator._sphereLocalScaleRadius;
        }

        float surfaceArea = 4f * Mathf.PI * largeSphereRadius * largeSphereRadius;
        float smallArea = surfaceArea / smallSphereCount;
        return Mathf.Sqrt(smallArea / (4f * Mathf.PI)) * smallSphereRadiusScale;
    }

    private int FindFirstUnassigned(bool[] assigned)
    {
        for (int i = 0; i < assigned.Length; i++)
            if (!assigned[i])
                return i;
        return -1;
    }

    public void GenerateFromJSON(SphereGenerator generator, SphereJSON json, Material[] materials)
    {
        _generator = generator;

        smallSphereCount = json.smallSphereCount;
        largeSphereRadius = json.largeSphereRadius;
        smallSphereRadiusScale = _generator._smallSphereRadiusScaleRuntime;

        ClearSpheres();
        GeneratePositions();

        float smallSphereRadius = CalculateSmallSphereRadius();

        for (int i = 0; i < smallSphereCount; i++)
        {
            GameObject sphere = UnityEngine.Object.Instantiate(generator._prefabSphere, generator.transform);
            sphere.transform.localPosition = spherePositions[i];
            sphere.transform.localScale = Vector3.one * (smallSphereRadius * 2f);
            sphere.GetComponent<Renderer>().material = materials[json.materialIndexes[i]];
            smallSpheres.Add(sphere);
        }
    }
}