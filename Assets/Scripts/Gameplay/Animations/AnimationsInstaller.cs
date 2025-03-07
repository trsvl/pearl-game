using Gameplay.Animations.StartAnimation;
using Gameplay.SphereData;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Animations
{
    public class AnimationsInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private ParticleSystem _onDestroySphereParticlePrefab;
        [SerializeField] private RectTransform _header;
        [SerializeField] private RectTransform _buttons;


        public void Install(IContainerBuilder builder)
        {
            builder.Register<DecreaseFOVAnimation>(Lifetime.Scoped);

            builder.Register<SpawnSmallSpheresAnimation>(Lifetime.Scoped)
                .WithParameter(resolver =>
                {
                    Transform parent = resolver.Resolve<SphereGenerator>().transform;
                    return parent;
                });

            builder.Register<InitializeThrowingBall>(Lifetime.Scoped);

            builder.Register<ParticlesFactory>(Lifetime.Scoped)
                .WithParameter(_onDestroySphereParticlePrefab);

            builder.Register<MoveUIAnimation>(Lifetime.Scoped)
                .WithParameter("header", _header)
                .WithParameter("buttons", _buttons);
        }
    }
}