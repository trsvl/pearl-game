using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.BallThrowing
{
    public class BallThrowerInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private BallThrower _ballThrower;


        public void Install(IContainerBuilder builder)
        {
            builder.Register<SphereOnHitBehaviour>(Lifetime.Scoped);

            builder.RegisterComponentOnNewGameObject<BallFactory>(Lifetime.Scoped, "Ball Factory")
                .WithParameter(_ballPrefab);

            builder.RegisterComponent(_ballThrower);
        }
    }
}