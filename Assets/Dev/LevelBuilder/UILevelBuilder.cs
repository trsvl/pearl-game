using Gameplay.SphereData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Dev.LevelBuilder
{
    public class UILevelBuilder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;

        [SerializeField] private Button _generateNewSphereButton;

        [SerializeField] private Button _loadSphereButton;
        [SerializeField] private Button _saveSphereButton;
        [SerializeField] private Button _deleteSphereButton;

        [SerializeField] private Button _prevLevelButton;
        [SerializeField] private Button _nextLevelButton;

        private SphereGeneratorBuilder _sphereGeneratorBuilder;
        private DataContextBuilder _dataContext;
        private int _levelNumber = 1;
        private bool _isNewLevel;


        [Inject]
        public void Init(SphereGeneratorBuilder sphereGeneratorBuilder, DataContextBuilder dataContext)
        {
            _sphereGeneratorBuilder = sphereGeneratorBuilder;
            _dataContext = dataContext;
            
            UpdateLevel(_levelNumber);
        }

        private void UpdateLevel(int newLevel)
        {
            _levelNumber = newLevel;

            CheckButtons();
            _levelText.SetText($"{(_isNewLevel ? "New Level " : "Level")} {_levelNumber}");

            LoadSpheresFromJSON();
        }

        private void CheckButtons()
        {
            _loadSphereButton.interactable = _dataContext.CheckFileExists(_levelNumber);
            _deleteSphereButton.interactable = _dataContext.CheckFileExists(_levelNumber);
            _prevLevelButton.interactable = _dataContext.CheckFileExists(_levelNumber - 1);

            if (_dataContext.CheckFileExists(_levelNumber))
            {
                _nextLevelButton.interactable = true;
                _isNewLevel = false;
            }
            else if (!_dataContext.CheckFileExists(_levelNumber + 1))
            {
                _nextLevelButton.interactable = false;
                _isNewLevel = true;
            }
            else
            {
                _nextLevelButton.interactable = false;
            }
        }

        private void GenerateNewSpheres()
        {
            _sphereGeneratorBuilder.GetSpheresData(true);
        }

        private void LoadSpheresFromJSON()
        {
            _ = _dataContext.LoadSpheres(_levelNumber, _sphereGeneratorBuilder);
        }

        private void SaveSpheresJSON()
        {
            SpheresData spheresData = _sphereGeneratorBuilder.GetSpheresData(false);
            _dataContext.SaveSpheresDataToJSON(spheresData, _levelNumber);

            UpdateLevel(_levelNumber);
        }

        private void DeleteSpheresJSON()
        {
            _dataContext.DeleteFile(_levelNumber);
        }

        private void OnEnable()
        {
            _generateNewSphereButton.onClick.AddListener(GenerateNewSpheres);

            _loadSphereButton.onClick.AddListener(LoadSpheresFromJSON);
            _saveSphereButton.onClick.AddListener(SaveSpheresJSON);

            _deleteSphereButton.onClick.AddListener(DeleteSpheresJSON);

            _prevLevelButton.onClick.AddListener(() => UpdateLevel(_levelNumber - 1));
            _nextLevelButton.onClick.AddListener(() => UpdateLevel(_levelNumber + 1));
        }

        private void OnDisable()
        {
            _generateNewSphereButton.onClick.RemoveAllListeners();

            _loadSphereButton.onClick.RemoveAllListeners();
            _saveSphereButton.onClick.RemoveAllListeners();
            _deleteSphereButton.onClick.RemoveAllListeners();

            _prevLevelButton.onClick.RemoveAllListeners();
            _nextLevelButton.onClick.RemoveAllListeners();
        }
    }
}