using System.Collections.Generic;
using UnityEngine;

namespace Utils.Colors
{
    public class AllColors
    {
        private readonly Dictionary<string, Color> allColors = new()
        {
            { "Red", new Color(1f, 0, 0) },
            { "Green", new Color(0, 1f, 0) },
            { "Blue", new Color(0, 0, 1f) },
            { "White", new Color(0.7f, 0.7f, 0.7f) },
            { "Black", new Color(0.1f, 0.1f, 0.1f) },
            { "Orange", new Color(1f, 0.7f, 0.1f) },
            { "Yellow", new Color(0.9f, 0.9f, 0.2f) },
        };
        public static readonly int BaseColor = Shader.PropertyToID("_Color");


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