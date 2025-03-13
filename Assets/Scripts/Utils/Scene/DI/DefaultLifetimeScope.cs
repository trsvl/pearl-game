using System.Threading;
using VContainer;

namespace Utils.Scene.DI
{
    public abstract class DefaultLifetimeScope : BaseLifetimeScope
    {
        private readonly CancellationTokenSource _cts = new();

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

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