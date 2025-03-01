using Utils.GameSystemLogic.Installers;

namespace Utils.GameSystemLogic.EntryPoint
{
    public class GameplayEntryPoint : EntryPoint
    {
        protected override void Awake()
        {
            base.Awake();

            container.GetService<GameplayStateObserver>().StartGame();
        }
    }
}