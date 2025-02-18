using System;

namespace Utils.SphereData
{
    [Serializable]
    public class SpheresJSON
    {
        public bool isStaticRadius;
        public float smallSphereRadius;
        public float smallSphereRadiusScale;
        public string[] colorNames;
        public SphereJSON[] spheres;
    }

    [Serializable]
    public class SphereJSON
    {
        public int smallSphereCount;
        public float largeSphereRadius;
        public int[] colorIndexes;
    }
}