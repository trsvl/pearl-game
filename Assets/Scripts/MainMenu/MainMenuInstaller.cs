using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

            sphereGenerator = new GameObject().AddComponent<SphereGenerator>();
            sphereGenerator.Init(spherePrefab);
            sphereGenerator.transform.position = new Vector3(0f, 3f, 0f);
        }

        private void Start()
        {
            levelNumber = PlayerData.Instance.CurrentLevel;
            UpdateLevel(levelNumber);
        }

        private void UpdateLevel(int newLevel)
        {
            levelNumber = newLevel;
            PlayerData.Instance.CurrentLevel = levelNumber;

            CheckButtons();
            dataContext.UpdateFilePath(levelNumber);
            levelText.SetText($"Level {levelNumber}");

            LoadSpheres();
        }

        private void CheckButtons()
        {
            previousLevelButton.interactable = dataContext.CheckFileExists(levelNumber - 1);
            nextLevelButton.interactable = dataContext.CheckFileExists(levelNumber + 1);
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
            sphereGenerator.LoadSpheresFromJSON(dataContext.LoadSpheresJSON());
        }
    }
}