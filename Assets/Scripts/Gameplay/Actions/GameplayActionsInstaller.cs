using Gameplay.BallThrowing;
using Gameplay.Header;
using Gameplay.SphereData;
using UnityEngine;
using Utils.GameSystemLogic.ContainerDI;

namespace Gameplay.Actions
{
    public class GameplayActionsInstaller : MonoBehaviour, IInstaller
    {
        private OnDestroySphereSegment onDestroySphereSegment;
        private OnDestroySphere onDestroySphere;


        public void Register(Container container)
        {
            onDestroySphereSegment = new OnDestroySphereSegment();
            onDestroySphere = new OnDestroySphere();

            var spheresDictionary = container.GetService<SpheresDictionary>();
            var sphereOnHitBehaviour = container.GetService<SphereOnHitBehaviour>();
            var pearlsData = container.GetService<PearlsData>();


            onDestroySphereSegment.SubscribeEvent((targetColor, targetSphere) =>
                spheresDictionary.DestroySpheresSegment(targetColor, targetSphere, onDestroySphere.NotifyAll));


            onDestroySphere.AddListener(pearlsData);
            onDestroySphere.SubscribeEvent(sphere => sphereOnHitBehaviour.ChangeSphere(sphere));
        }

        public void OnDestroy()
        {
            onDestroySphereSegment.RemoveAllListeners();
            onDestroySphere.RemoveAllListeners();
        }

        private void DestroySphereSegmentAddListeners(Container container)
        {
        }
    }
}