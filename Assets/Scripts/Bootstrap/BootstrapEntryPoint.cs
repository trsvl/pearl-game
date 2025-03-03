using System.Threading;
using System.Threading.Tasks;
using VContainer.Unity;

namespace Bootstrap
{
    public class BootstrapEntryPoint : IAsyncStartable
    {
        private readonly Loader _loader;


        public BootstrapEntryPoint(Loader loader)
        {
            _loader = loader;
        }

        public async Task StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            await _loader.LoadScene(SceneName.MainMenu);
        }
    }
}