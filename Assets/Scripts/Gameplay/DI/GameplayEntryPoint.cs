using System.Threading;
using Bootstrap;
using Cysharp.Threading.Tasks;
using Gameplay.Animations;
using Gameplay.SphereData;
using Gameplay.UI.Header;
using Gameplay.Utils;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayEntryPoint : IAsyncStartable
    {
        private readonly IObjectResolver _container;


        public GameplayEntryPoint(IObjectResolver container)
        {
            _container = container;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var dataContext = _container.Resolve<DataContext>();
            var playerData = _container.Resolve<PlayerData>();
            var sphereGenerator = _container.Resolve<SphereGenerator>();
            var spheresDictionary = _container.Resolve<SpheresDictionary>();

            await dataContext.LoadSpheres(playerData.CurrentLevel, sphereGenerator);

            await _container.Resolve<SpawnSmallSpheresAnimation>().DoAnimation();

            _container.Resolve<ShotsData>().SetInitialNumber(spheresDictionary.GetShotsCount());

            _container.Resolve<MoveUIAnimation>().MoveOnStart();

            await _container.Resolve<ThrowingBallAnimation>().DoAnimation();

            _container.Resolve<GameplayStateObserver>().StartGame();
        }
    }
}