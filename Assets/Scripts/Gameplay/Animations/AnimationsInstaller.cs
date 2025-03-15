using Gameplay.SphereData;
using MainMenu.UI.Header;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Animations
{
    public class AnimationsInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private ParticleSystem _onDestroySphereParticlePrefab;
        [SerializeField] private RectTransform _header;
        [SerializeField] private RectTransform _pauseButton;
        [SerializeField] private RectTransform _respawnBallButton;
        [SerializeField] private Camera _uiCamera;


        public void Install(IContainerBuilder builder)
        {
            builder.Register<CameraManager>(Lifetime.Scoped)
                .WithParameter(_uiCamera);

            builder.Register<DecreaseFOVAnimation>(Lifetime.Scoped);

            builder.Register<SpawnSmallSpheresAnimation>(Lifetime.Scoped)
                .WithParameter(resolver =>
                {
                    Transform parent = resolver.Resolve<SphereGenerator>().transform;
                    return parent;
                });

            builder.Register<ThrowingBallAnimation>(Lifetime.Scoped)
                .WithParameter("respawnBallButton", _respawnBallButton);

            builder.RegisterComponentOnNewGameObject<ParticlesFactory>(Lifetime.Scoped, "Particles Factory")
                .WithParameter(_onDestroySphereParticlePrefab);

            builder.Register<ChangeHeader>(Lifetime.Scoped)
                .WithParameter("header", _header);

            builder.Register<MoveUIAnimation>(Lifetime.Scoped)
                .WithParameter("header", _header)
                .WithParameter("pauseButton", _pauseButton);
        }
    }
}