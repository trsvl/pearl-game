using Utils.EventBusSystem;
using Utils.Scene;
using VContainer;

namespace MainMenu.DI
{
    public class MainMenuEventsHandler : SceneEventsHandler
    {
        public MainMenuEventsHandler(EventBus eventBus, IObjectResolver container) : base(eventBus,
            container, new[]
            {
                typeof(MainMenuAudioEventsHandler),
            })
        {
        }
    }
}