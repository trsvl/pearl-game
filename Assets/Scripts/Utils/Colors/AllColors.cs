using System.Collections.Generic;
using UnityEngine;

namespace Utils.Colors
{
    public class AllColors
    {
        private readonly Dictionary<string, Color> allColors = new()
        {
            { "Red", new Color(255, 0, 0, 255) },
            { "Green", new Color(0, 255, 0, 255) },
            { "Blue", new Color(0, 0, 255, 255) },
            { "White", new Color(255, 255, 255, 255) },
            { "Black", new Color(0, 0, 0, 255) },
            { "Orange", new Color(250, 156, 28, 255) },
            { "Yellow", new Color(234, 239, 44, 255) },
        };
        public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");


        public Color[] GetAllColors()
        {
            Color[] colors = new Color[allColors.Count];
            allColors.Values.CopyTo(colors, 0);
            return colors;
        }

        public Color GetColor(string colorName)
        {
            return allColors.GetValueOrDefault(colorName);
        }
    }
}