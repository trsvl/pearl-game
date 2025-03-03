using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.SphereData
{
    public class SphereGeneratorInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameObject spherePrefab;

        public void Install(IContainerBuilder builder)
        {
            builder.Register<AllColors>(Lifetime.Scoped);

            builder.Register<SpheresDictionary>(Lifetime.Scoped);

            builder.Register<DataContext>(Lifetime.Scoped);

            builder.Register<GameplayStateObserver>(Lifetime.Scoped);

            builder.RegisterComponentOnNewGameObject<SphereGenerator>(Lifetime.Scoped, "Sphere Generator")
                .WithParameter(spherePrefab);
        }
    }
}