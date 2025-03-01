using UnityEngine;
using Utils.GameSystemLogic.ContainerDI;

namespace Utils.GameSystemLogic.Installers
{
    public class MonoBehaviourStateObserverInstaller : MonoBehaviour, IInstaller, IStateObserverInstaller
    {
        private MonoBehaviourStateObserver _stateObserver;


        public IStateObserver RegisterStateObserver()
        {
            var gameObj = new GameObject("MonoBehaviourStateObserver");
            var monoBehaviourStateObserver = gameObj.AddComponent<MonoBehaviourStateObserver>();
            _stateObserver = monoBehaviourStateObserver;
            return _stateObserver;
        }

        public void Register(Container container)
        {
            container.Bind(_stateObserver, isSingleton: true);
        }
    }
}