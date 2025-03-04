using VContainer.Unity;

namespace MainMenu.DI
{
    public class MainMenuEntryPoint : IStartable
    {
        private readonly MainMenuManager _mainMenuManager;


        public MainMenuEntryPoint(MainMenuManager mainMenuManager)
        {
            _mainMenuManager = mainMenuManager;
        }

        public void Start()
        {
            _mainMenuManager.UpdateLevel();
        }
    }
}