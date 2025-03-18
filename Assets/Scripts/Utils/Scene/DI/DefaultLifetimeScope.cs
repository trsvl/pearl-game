using System.Threading;
using Bootstrap;
using UnityEngine;
using VContainer;

namespace Utils.Scene.DI
{
    public abstract class DefaultLifetimeScope : BaseLifetimeScope
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _perspectiveCamera;
        [SerializeField] private Camera _uiCamera;

        private readonly CancellationTokenSource _cts = new();


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(_cts.Token);
            Parent.Container.Resolve<CameraController>().UpdateCameras(_mainCamera, _perspectiveCamera, _uiCamera);
        }

        protected override void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();

            base.OnDestroy();
        }
    }
}