using System.Threading;
using System.Threading.Tasks;
using Bootstrap;
using Gameplay.Animations;
using Gameplay.BallThrowing;
using Gameplay.SphereData;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;
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

        public async Task StartAsync(CancellationToken cancellation = new())
        {
            var dataContext = _container.Resolve<DataContext>();
            var playerData = _container.Resolve<PlayerData>();
            var sphereGenerator = _container.Resolve<SphereGenerator>();

            await dataContext.LoadSpheres(playerData.CurrentLevel, sphereGenerator);

            await _container.Resolve<SpawnSmallSpheresAnimation>().DoAnimation();

            _ = _container.Resolve<MoveUIAnimation>().DoAnimation();

            await _container.Resolve<ThrowingBallAnimation>().DoAnimation();

            _container.Resolve<GameplayStateObserver>().StartGame();
        }
    }
}