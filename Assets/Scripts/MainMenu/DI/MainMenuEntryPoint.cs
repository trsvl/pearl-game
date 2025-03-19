using MainMenu.UI.Footer;
using MainMenu.UI.Header;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace MainMenu.DI
{
    public class MainMenuEntryPoint : IInitializable
    {
        private readonly IObjectResolver _container;


        public MainMenuEntryPoint(IObjectResolver container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.Resolve<MainMenuHeaderManager>().InitHeader();
            _container.Resolve<MainMenuFooter>().UpdateLevel();
            _container.Resolve<EventBus>().RaiseEvent<IMainMenuStart>(handler => handler.OnMainMenuStart());
        }
    }
}