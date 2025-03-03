using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Header
{
    public class HeaderInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private TextMeshProUGUI pearlsText;
        [SerializeField] private TextMeshProUGUI shotsText;

        public void Install(IContainerBuilder builder)
        {
            builder.Register<PearlsData>(Lifetime.Scoped)
                .WithParameter(pearlsText);
            builder.Register<ShotsData>(Lifetime.Scoped)
                .WithParameter(shotsText)
                .WithParameter(5); //!!!
        }
    }
}