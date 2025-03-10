﻿using System.Collections.Generic;
using Gameplay.Animations;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private List<GameObject> _installers;


        protected override void Configure(IContainerBuilder builder)
        {
            foreach (GameObject gameObj in _installers)
            {
                var installer = gameObj.GetComponent<IInstaller>();
                installer?.Install(builder);
            }

            new UtilsInstaller().Install(builder);

            builder.UseEntryPoints(points =>
            {
                points.Add<GameplaySubscriber>();
                points.Add<GameplayEntryPoint>();
            });
        }
    }
}