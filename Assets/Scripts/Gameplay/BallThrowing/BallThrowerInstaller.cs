using Gameplay.Actions;
using Gameplay.Header;
using Gameplay.SphereData;
using UnityEngine;
using Utils.GameSystemLogic.ContainerDI;

namespace Gameplay.BallThrowing
{
    public class BallThrowerInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private BallThrower _ballThrower;


        public void Register(Container container)
        {
            var spheresDictionary = container.GetService<SpheresDictionary>();
            Vector3 lowestSphereScale = spheresDictionary.GetLowestSphereScale();

            var shotsData = container.GetService<ShotsData>();
            var onDestroySphereSegment = container.GetService<OnDestroySphereSegment>();


            var ballFactory = new GameObject("BallFactory").AddComponent<BallFactory>();
            ballFactory.Init(_ballPrefab, shotsData, onDestroySphereSegment, lowestSphereScale);
            _ballThrower.Init(ballFactory);

            container.Bind(ballFactory, isSingleton: true);
            container.Bind(_ballThrower, isSingleton: true);
        }
    }
}