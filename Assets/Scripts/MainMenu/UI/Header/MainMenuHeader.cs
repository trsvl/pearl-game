using TMPro;
using UnityEngine;

namespace MainMenu.UI.Header
{
    public class MainMenuHeader : MonoBehaviour
    {
        public RectTransform _goldIcon;
        public TextMeshProUGUI _goldText;
        public RectTransform _diamondIcon;
        public TextMeshProUGUI _diamondText;


        public void AssignCamera(Camera worldCamera)
        {
            GetComponent<Canvas>().worldCamera = worldCamera;
        }
    }
}