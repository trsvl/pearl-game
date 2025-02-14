using System.Collections;
using _Project.Scripts.Utils.UI.Buttons;
using Gameplay.BallThrowing;
using Gameplay.Header;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.PlayerData;
using Utils.SphereData;

namespace Gameplay
{
    public class GameplayInstaller : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pearlsText;
        [SerializeField] private TextMeshProUGUI shotsText;
        [SerializeField] private BallThrower ballThrower;
        [SerializeField] private SphereDestroyer sphereDestroyer;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button changeBallButton;

        [Header("Popup")] [Space] [SerializeField]
        private TextButton firstButton;

        [SerializeField] private TextButton secondButton;
        [SerializeField] private GameObject popup;

        private GameplayStateObserver gameplayStateObserver;
        private GamePopup gamePopup;
        private SphereGenerator sphereGenerator;
        private DataContext dataContext;
        private PearlsData pearlsData;
        private ShotsData shotsData;


        private IEnumerator Start()
        {
            var spherePrefab = Resources.Load<GameObject>("Prefabs/Sphere");
            dataContext = new DataContext();
            gameplayStateObserver = new GameplayStateObserver();
            pearlsData = new PearlsData(pearlsText);
            gamePopup = new GamePopup(popup, firstButton, secondButton, gameplayStateObserver);

            sphereGenerator = new GameObject().AddComponent<SphereGenerator>();
            sphereGenerator.Init(spherePrefab, gameplayStateObserver);
            sphereGenerator.transform.position = new Vector3(0f, 3f, 0f);

            int level = PlayerData.Instance.CurrentLevel;
            yield return StartCoroutine(dataContext.LoadSpheres(level, sphereGenerator));

            int shotsCount = sphereGenerator._materials.Length * 2;
            shotsData = new ShotsData(shotsText, shotsCount, gameplayStateObserver);

            ballThrower.Init(sphereGenerator._materials, shotsData, gameplayStateObserver);
            sphereDestroyer.Init(pearlsData);

            gameplayStateObserver.AddListener(gamePopup);

            gameplayStateObserver.StartGame();
        }

        private void OnEnable()
        {
            pauseButton.onClick.AddListener(() => gameplayStateObserver.PauseGame());
            changeBallButton.onClick.AddListener(() => ballThrower.RespawnBall());
        }

        private void OnDisable()
        {
            pauseButton.onClick.RemoveAllListeners();
            changeBallButton.onClick.RemoveAllListeners();
        }
    }
}