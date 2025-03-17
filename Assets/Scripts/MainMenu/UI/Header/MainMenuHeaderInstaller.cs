using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainMenu.UI.Header
{
    public class MainMenuHeaderInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private MainMenuHeader _mainMenuHeaderPrefab;
        [SerializeField] private Camera _uiCamera;

        public void Install(IContainerBuilder builder)
        {
            builder.Register<MainMenuHeaderManager>(Lifetime.Scoped)
                .WithParameter(_mainMenuHeaderPrefab)
                .WithParameter(_uiCamera);
        }
    }
}