using Gameplay.SphereData;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Dev.LevelBuilder
{
    public class LevelBuilderInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameObject spherePrefab;

        public void Install(IContainerBuilder builder)
        {
            builder.Register<AllColors>(Lifetime.Scoped);

            builder.Register<SpheresDictionary>(Lifetime.Scoped);

            builder.Register<DataContextBuilder>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<SphereGeneratorBuilder>()
                .WithParameter(spherePrefab);
        }
    }
}