using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.SphereData;

namespace Dev.LevelBuilder
{
    public class GeneratorInstaller : MonoBehaviour
    {
        [SerializeField] private SphereGeneratorBuilder sphereGenerator;
        [SerializeField] private TextMeshProUGUI levelText;

        [SerializeField] private Button generateNewSphereButton;

        [SerializeField] private Button loadSphereButton;
        [SerializeField] private Button saveSphereButton;
        [SerializeField] private Button deleteSphereButton;

        [SerializeField] private Button prevLevelButton;
        [SerializeField] private Button nextLevelButton;

        private DataContextBuilder dataContext;
        private int levelNumber = 1;
        private bool isNewLevel;


        private void Awake()
        {
            var spherePrefab = Resources.Load<GameObject>("Prefabs/Sphere");
            var allColors = new AllColors();
            var allSpheres = new AllSpheresData();

            dataContext = new DataContextBuilder();
            sphereGenerator.Init(spherePrefab, allColors, allSpheres);
            UpdateLevel(levelNumber);
        }

        private void UpdateLevel(int newLevel)
        {
            levelNumber = newLevel;

            CheckButtons();
            levelText.SetText($"{(isNewLevel ? "New Level " : "Level")} {levelNumber}");

            LoadSpheresFromJSON();
        }

        private void CheckButtons()
        {
            loadSphereButton.interactable = dataContext.CheckFileExists(levelNumber);
            deleteSphereButton.interactable = dataContext.CheckFileExists(levelNumber);
            prevLevelButton.interactable = dataContext.CheckFileExists(levelNumber - 1);

            if (dataContext.CheckFileExists(levelNumber))
            {
                nextLevelButton.interactable = true;
                isNewLevel = false;
            }
            else if (!dataContext.CheckFileExists(levelNumber + 1))
            {
                nextLevelButton.interactable = false;
                isNewLevel = true;
            }
            else
            {
                nextLevelButton.interactable = false;
            }
        }

        private void GenerateNewSpheres()
        {
            sphereGenerator.GenerateNewSpheres();
        }

        private void LoadSpheresFromJSON()
        {
            StartCoroutine(dataContext.LoadSpheres(levelNumber, sphereGenerator));
        }

        private void SaveSpheresJSON()
        {
            (SphereGeneratorBuilder generator, string[] newColorNames, List<int[]> newColorsIds) =
                sphereGenerator.GetDataForJSON();
            dataContext.SaveSpheresJSON(generator, newColorNames, newColorsIds, levelNumber);

            UpdateLevel(0);
        }

        private void DeleteSpheresJSON()
        {
            dataContext.DeleteFile(levelNumber);
        }

        private void OnEnable()
        {
            generateNewSphereButton.onClick.AddListener(GenerateNewSpheres);

            loadSphereButton.onClick.AddListener(LoadSpheresFromJSON);
            saveSphereButton.onClick.AddListener(SaveSpheresJSON);

            deleteSphereButton.onClick.AddListener(DeleteSpheresJSON);

            prevLevelButton.onClick.AddListener(() => UpdateLevel(levelNumber - 1));
            nextLevelButton.onClick.AddListener(() => UpdateLevel(levelNumber + 1));
        }

        private void OnDisable()
        {
            generateNewSphereButton.onClick.RemoveAllListeners();

            loadSphereButton.onClick.RemoveAllListeners();
            saveSphereButton.onClick.RemoveAllListeners();
            deleteSphereButton.onClick.RemoveAllListeners();

            prevLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
        }
    }
}