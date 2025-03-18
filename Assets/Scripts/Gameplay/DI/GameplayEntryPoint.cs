using System.Threading;
using Bootstrap;
using Cysharp.Threading.Tasks;
using Gameplay.Animations;
using Gameplay.SphereData;
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

            await dataContext.LoadSpheres(playerData.CurrentLevel, sphereGenerator);

            await _container.Resolve<SpawnSmallSpheresAnimation>().DoAnimation();

            _container.Resolve<MoveUIAnimation>().MoveOnStart().Forget();

            await _container.Resolve<ThrowingBallAnimation>().DoAnimation();

            _container.Resolve<GameplayStateObserver>().StartGame();

            await UniTask.WaitForSeconds(0.1f, cancellationToken: cancellation, ignoreTimeScale: true);

            _container.Resolve<GameplayStateObserver>().FinishGame();
        }
    }
}