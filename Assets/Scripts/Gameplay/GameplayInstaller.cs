using System.Collections;
using _Project.Scripts.Utils.UI.Buttons;
using Gameplay.BallThrowing;
using Gameplay.Effects;
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
        [SerializeField] private SphereOnHitBehaviour _sphereOnHitBehaviour;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button changeBallButton;

        [Header("Popup")] [Space] [SerializeField]
        private TextButton firstButton;

        [SerializeField] private TextButton secondButton;
        [SerializeField] private GameObject popup;

        private GameplayStateObserver gameplayStateObserver;


        private IEnumerator Start()
        {
            var spherePrefab = Resources.Load<GameObject>("Prefabs/Sphere");
            var dataContext = new DataContext();
            gameplayStateObserver = new GameplayStateObserver();
            var pearlsData = new PearlsData(pearlsText);
            var gamePopup = new GamePopup(popup, firstButton, secondButton, gameplayStateObserver);
            var allColors = new AllColors();
            var spheresDictionary = new SpheresDictionary();
            var spawnAnimation = new SpawnSmallSpheresAnimation();

            var sphereGenerator = new GameObject().AddComponent<SphereGenerator>();
            sphereGenerator.Init(spherePrefab, allColors, spheresDictionary);
            sphereGenerator.transform.position = new Vector3(0f, 1f, 40f);

            int level = PlayerData.Instance.CurrentLevel;
            yield return StartCoroutine(dataContext.LoadSpheres(level, sphereGenerator));

            int shotsCount = sphereGenerator._levelColors.Length * 2;
            var shotsData = new ShotsData(shotsText, shotsCount, gameplayStateObserver);

            ballThrower.Init(sphereGenerator._levelColors, shotsData, spheresDictionary);
            _sphereOnHitBehaviour.Init(pearlsData, spheresDictionary);

            gameplayStateObserver.AddListener(sphereGenerator);
            gameplayStateObserver.AddListener(ballThrower);
            gameplayStateObserver.AddListener(gamePopup);

            yield return StartCoroutine(
                spawnAnimation.MoveSpheresToCenter(spheresDictionary, sphereGenerator.transform));

            gameplayStateObserver.StartGame();
        }

        private void UpdateCameraFOV()
        {
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