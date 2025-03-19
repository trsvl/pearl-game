using Bootstrap;
using Cysharp.Threading.Tasks;
using Gameplay.SphereData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI.Buttons;
using VContainer;

namespace MainMenu.UI.Footer
{
    public class MainMenuFooter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private BaseButton _startGameButton;
        [SerializeField] private BaseButton _previousLevelButton;
        [SerializeField] private BaseButton _nextLevelButton;

        private PlayerData _playerData;
        private SphereGenerator _sphereGenerator;
        private DataContext _dataContext;
        private Loader _loader;
        private int _levelNumber;


        [Inject]
        public void Init(PlayerData playerData, SphereGenerator sphereGenerator, DataContext dataContext,
            Loader loader)
        {
            _playerData = playerData;
            _sphereGenerator = sphereGenerator;
            _dataContext = dataContext;
            _loader = loader;

            _levelNumber = _playerData.CurrentLevel;
        }

        public void UpdateLevel(int newLevel = 0)
        {
            _levelNumber = newLevel > 0 ? newLevel : _levelNumber;
            _playerData.CurrentLevel = _levelNumber;

            CheckButtons();
            _levelText.SetText($"Level {_levelNumber}");

            _ = _dataContext.LoadSpheres(_levelNumber, _sphereGenerator);
        }

        private void CheckButtons()
        {
            _previousLevelButton.interactable = 1 <= _levelNumber - 1;
            _nextLevelButton.interactable = _playerData.MaxLevel >= _levelNumber + 1;
        }

        private void OnEnable()
        {
            _startGameButton.Init(()=>_loader.LoadScene(SceneName.Gameplay).Forget());
            _previousLevelButton.Init(() => UpdateLevel(_levelNumber - 1));
            _nextLevelButton.Init(() => UpdateLevel(_levelNumber + 1));
        }

        private void OnDisable()
        {
            _startGameButton.onClick.RemoveAllListeners();
            _previousLevelButton.onClick.RemoveAllListeners();
            _nextLevelButton.onClick.RemoveAllListeners();
        }
    }
}