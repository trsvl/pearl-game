using System;

namespace Gameplay.SphereData
{
    [Serializable]
    public class SpheresData
    {
        public string[] colorNames;
        public BigSphereData[] spheres;
    }

    [Serializable]
    public class BigSphereData
    {
        public int smallSphereCount;
        public float largeSphereRadius;
        public int[] colorIndexes;
    }
}