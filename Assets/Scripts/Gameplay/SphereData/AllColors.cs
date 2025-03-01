using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.SphereData
{
    public class AllColors
    {
        private readonly Dictionary<string, Color> allColors = new()
        {
            { ColorName.Red.ToString(), new Color(1f, 0, 0) },
            { ColorName.Green.ToString(), new Color(0, 1f, 0) },
            { ColorName.Blue.ToString(), new Color(0, 0, 1f) },
            { ColorName.White.ToString(), new Color(0.7f, 0.7f, 0.7f) },
            { ColorName.Black.ToString(), new Color(0.1f, 0.1f, 0.1f) },
            { ColorName.Orange.ToString(), new Color(1f, 0.7f, 0.1f) },
            { ColorName.Yellow.ToString(), new Color(0.9f, 0.9f, 0.2f) },
        };

        public static readonly int BaseColor = Shader.PropertyToID("_Color");


        public Color GetColor(string colorName)
        {
            return allColors.GetValueOrDefault(colorName);
        }
    }
}