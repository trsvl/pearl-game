using Gameplay.Animations.StartAnimation;
using Gameplay.SphereData;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Animations
{
    public class AnimationsInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<DecreaseFOVAnimation>(Lifetime.Scoped);

            builder.Register<SpawnSmallSpheresAnimation>(Lifetime.Scoped)
                .WithParameter(resolver =>
                {
                    Transform parent = resolver.Resolve<SphereGenerator>().transform;
                    return parent;
                });

            builder.Register<MoveThrowingBall>(Lifetime.Scoped);
        }
    }
}