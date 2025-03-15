using Gameplay.Animations;
using Gameplay.BallThrowing;
using Gameplay.SphereData;
using Gameplay.UI.Buttons;
using Gameplay.UI.Header;
using Gameplay.UI.Popup;
using Gameplay.Utils;
using Utils.EventBusSystem;
using Utils.Scene;
using VContainer;

namespace Gameplay.DI
{
    public class GameplayEventsHandler : SceneEventsHandler
    {
        public GameplayEventsHandler(EventBus eventBus, IObjectResolver container) : base(eventBus,
            container, new[]
            {
                typeof(PearlsData),
                typeof(SphereOnHitBehaviour),
                typeof(SpheresDictionary),
                typeof(ParticlesFactory),
                typeof(ThrowingBallAnimation),
                typeof(RespawnBallButton),
                typeof(GameResultChecker),
                typeof(DecreaseFOVAnimation),
                typeof(GameplayAudioEventsHandler),
                typeof(SpawnSmallSpheresAnimation),
                typeof(GamePopupManager),
                typeof(PauseButton),
                typeof(SphereGenerator),
                typeof(BallThrower),
                typeof(FinishGameController),
            })
        {
        }
    }
}