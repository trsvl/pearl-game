using UnityEngine;
using Utils.Singleton;

namespace Utils.Bootstrap.PlayerData
{
    public class PlayerData : Singleton<PlayerData>
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

        public int MaxLevel { get; private set; }

        private int currentLevel;
        private const string CURRENT_LEVEL = "CurrentLevel";


        public void Init()
        {
            currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL, 1);

            MaxLevel = 5; //!!!
        }
    }
}