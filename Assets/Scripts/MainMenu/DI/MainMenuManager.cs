using System.Collections;
using Bootstrap;
using Gameplay.SphereData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MainMenu.DI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _previousLevelButton;
        [SerializeField] private Button _nextLevelButton;

        private  PlayerData _playerData;
        private  SphereGenerator _sphereGenerator;
        private  DataContext _dataContext;
        private  Loader _loader;
        private int _levelNumber;


        [Inject]
        public void Init(PlayerData playerData, SphereGenerator sphereGenerator, DataContext dataContext,
            Loader loader)
        {
            Debug.LogError("Test");
            _playerData = playerData;
            _sphereGenerator = sphereGenerator;
            _dataContext = dataContext;
            _loader = loader;

            _levelNumber = _playerData.CurrentLevel;
            UpdateLevel(_levelNumber);
        }

        private void UpdateLevel(int newLevel)
        {
            _levelNumber = newLevel;
            _playerData.CurrentLevel = _levelNumber;

            CheckButtons();
            _levelText.SetText($"Level {_levelNumber}");

            StartCoroutine(LoadSpheres());
        }

        private void CheckButtons()
        {
            _previousLevelButton.interactable = 1 <= _levelNumber - 1;
            _nextLevelButton.interactable = _playerData.MaxLevel >= _levelNumber + 1;
        }

        private void OnEnable()
        {
            _startGameButton.onClick.AddListener(() => _ = _loader.LoadScene(SceneName.Gameplay));
            _previousLevelButton.onClick.AddListener(() => UpdateLevel(_levelNumber - 1));
            _nextLevelButton.onClick.AddListener(() => UpdateLevel(_levelNumber + 1));
        }

        private void OnDisable()
        {
            _startGameButton.onClick.RemoveAllListeners();
            _previousLevelButton.onClick.RemoveAllListeners();
            _nextLevelButton.onClick.RemoveAllListeners();
        }

        private IEnumerator LoadSpheres()
        {
            for (int i = 0; i < _sphereGenerator.transform.childCount; i++)
            {
                DestroyImmediate(_sphereGenerator.transform.GetChild(i).gameObject);
            }

            yield return null;

            StartCoroutine(_dataContext.LoadSpheres(_levelNumber, _sphereGenerator));
        }
    }
}