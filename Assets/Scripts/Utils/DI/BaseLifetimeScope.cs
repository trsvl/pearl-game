using System.Threading;
using VContainer;
using VContainer.Unity;

namespace Utils.DI
{
    public abstract class BaseLifetimeScope : LifetimeScope
    {
        private readonly CancellationTokenSource _cts = new();

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_cts.Token);
        }

        protected override void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
            base.OnDestroy();
        }
    }
}