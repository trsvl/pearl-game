using UnityEngine;

namespace Utils.Bootstrap.PlayerData
{
    public class PlayerDataInstaller : MonoBehaviour, IBootstrapInstaller
    {
        public void Load()
        {
            var playerData = new GameObject().AddComponent<PlayerData>();
            playerData.gameObject.name = "PlayerData";
            playerData.Init();
        }
    }
}