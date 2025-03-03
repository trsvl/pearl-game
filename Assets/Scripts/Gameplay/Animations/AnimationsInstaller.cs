using Gameplay.Animations.EntryPoint;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Animations
{
    public class AnimationsInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<CameraManager>(Lifetime.Scoped);
            builder.Register<DecreaseFOVAnimation>(Lifetime.Scoped);
            builder.Register<SpawnSmallSpheresAnimation>(Lifetime.Scoped);
        }
    }
}