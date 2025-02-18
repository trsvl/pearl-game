using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Colors;
using Utils.Loader;
using Utils.PlayerData;
using Utils.SphereData;

namespace MainMenu
{
    public class MainMenuInstaller : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button previousLevelButton;
        [SerializeField] private Button nextLevelButton;

        private SphereGenerator sphereGenerator;
        private DataContext dataContext;
        private int levelNumber;


        private void Awake()
        {
            var spherePrefab = Resources.Load<GameObject>("Prefabs/Sphere");
            dataContext = new DataContext();
            var allColors = new AllColors();
            var allSpheres = new AllSpheres();

            sphereGenerator = new GameObject().AddComponent<SphereGenerator>();
            sphereGenerator.Init(spherePrefab, allColors, allSpheres);
            sphereGenerator.transform.position = new Vector3(0f, 3f, 0f);

            levelNumber = PlayerData.Instance.CurrentLevel;
            UpdateLevel(levelNumber);
        }

        private void UpdateLevel(int newLevel)
        {
            levelNumber = newLevel;
            PlayerData.Instance.CurrentLevel = levelNumber;

            CheckButtons();
            levelText.SetText($"Level {levelNumber}");

            LoadSpheres();
        }

        private void CheckButtons()
        {
            var playerData = PlayerData.Instance;

            previousLevelButton.interactable = 1 <= levelNumber - 1;
            nextLevelButton.interactable = playerData.MaxLevel >= levelNumber + 1;
        }

        private void OnEnable()
        {
            startGameButton.onClick.AddListener(() => _ = Loader.Instance.LoadScene(SceneName.Gameplay));
            previousLevelButton.onClick.AddListener(() => UpdateLevel(levelNumber - 1));
            nextLevelButton.onClick.AddListener(() => UpdateLevel(levelNumber + 1));
        }

        private void OnDisable()
        {
            startGameButton.onClick.RemoveAllListeners();
            previousLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
        }

        private void LoadSpheres()
        {
            StartCoroutine(dataContext.LoadSpheres(levelNumber, sphereGenerator));
        }
    }
}