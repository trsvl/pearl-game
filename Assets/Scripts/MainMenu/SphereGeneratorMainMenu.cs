using UnityEngine;
using Utils.SphereData;

namespace MainMenu
{
    public class SphereGeneratorMainMenu : SphereGenerator
    {
        public override void LoadSpheresFromJSON(SpheresJSON json)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            base.LoadSpheresFromJSON(json);
        }
    }
}