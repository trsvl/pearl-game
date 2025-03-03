using UnityEngine;

namespace Bootstrap
{
    public class PlayerData
    {
        public int CurrentLevel
        {
            get => currentLevel;
            set
            {
                currentLevel = value;
                PlayerPrefs.SetInt(CURRENT_LEVEL, currentLevel);
            }
        }

        public int MaxLevel { get; private set; } = 5; //!!!

        private int currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL, 1);
        private const string CURRENT_LEVEL = "CurrentLevel";
    }
}