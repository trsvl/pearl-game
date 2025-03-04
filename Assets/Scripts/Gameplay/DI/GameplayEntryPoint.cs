using System;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap;
using Gameplay.Animations.StartAnimation;
using Gameplay.BallThrowing;
using Gameplay.SphereData;
using Gameplay.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayEntryPoint : IAsyncStartable
    {
        private readonly IObjectResolver _resolver;


        public GameplayEntryPoint(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task StartAsync(CancellationToken cancellation = new())
        {
            var dataContext = _resolver.Resolve<DataContext>();
            var playerData = _resolver.Resolve<PlayerData>();
            var sphereGenerator = _resolver.Resolve<SphereGenerator>();

            await dataContext.LoadSpheres(playerData.CurrentLevel, sphereGenerator);

            InitBallFactory();

            await _resolver.Resolve<SpawnSmallSpheresAnimation>().DoAnimation();
            await _resolver.Resolve<MoveThrowingBall>().DoAnimation();
            Debug.LogError("Gameplay started");

            _resolver.Resolve<GameplayStateObserver>().StartGame();
        }

        private void InitBallFactory()
        {
            var sphereLowestScale = _resolver.Resolve<SpheresDictionary>().LowestSphereScale;
            var levelColors = _resolver.Resolve<SphereGenerator>()._levelColors;
            _resolver.Resolve<BallFactory>().ReInit(sphereLowestScale, levelColors);
        }
    }
}