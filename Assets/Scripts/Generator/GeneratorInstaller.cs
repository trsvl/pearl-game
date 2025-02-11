using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Generator
{
    public class GeneratorInstaller : MonoBehaviour
    {
        [SerializeField] private SphereGenerator generator;
        [SerializeField] private TextMeshProUGUI levelText;

        [SerializeField] private Button generateNewSphereButton;

        [SerializeField] private Button loadSphereButton;
        [SerializeField] private Button saveSphereButton;
        [SerializeField] private Button deleteSphereButton;

        [SerializeField] private Button prevLevelButton;
        [SerializeField] private Button nextLevelButton;

        private DataContext dataContext;
        private int levelNumber = 1;
        private bool isNewLevel = false;


        private void Awake()
        {
            dataContext = new DataContext();
            UpdateLevel(0);
        }

        private void UpdateLevel(int number)
        {
            levelNumber += number;

            CheckButtons();

            dataContext.UpdateFilePath(levelNumber);
            levelText.SetText(isNewLevel ? $"New Level {levelNumber}" : $"Level {levelNumber}");
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
            generator.GenerateNewSpheres();
        }

        private void LoadSpheresJSON()
        {
            generator.LoadSpheresFromJSON(dataContext.LoadSpheresJSON());
        }

        private void SaveSpheresJSON()
        {
            (
                SphereGenerator sphereGenerator,
                string[] newMaterialNames,
                List<int[]> materialIndexesList) = generator.GetDataForJSON();
            dataContext.SaveSpheresJSON(sphereGenerator, newMaterialNames, materialIndexesList);

            UpdateLevel(0);
        }

        private void DeleteSpheresJSON()
        {
            dataContext.DeleteFile();
        }

        private void OnEnable()
        {
            generateNewSphereButton.onClick.AddListener(GenerateNewSpheres);

            loadSphereButton.onClick.AddListener(LoadSpheresJSON);
            saveSphereButton.onClick.AddListener(SaveSpheresJSON);

            deleteSphereButton.onClick.AddListener(DeleteSpheresJSON);

            prevLevelButton.onClick.AddListener(() => UpdateLevel(-1));
            nextLevelButton.onClick.AddListener(() => UpdateLevel(1));
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