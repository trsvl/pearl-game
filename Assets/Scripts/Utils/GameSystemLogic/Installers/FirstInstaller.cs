using UnityEngine;
using Utils.GameSystemLogic.ContainerDI;

namespace Utils.GameSystemLogic.Installers
{
    public class FirstInstaller : MonoBehaviour, IFirstInstaller
    {
        [SerializeField] private MonoBehaviour[] _stateObserverInstallers;


        public void Register(Container container)
        {
            foreach (var monoBehaviour in _stateObserverInstallers)
            {
                if (monoBehaviour is IStateObserverInstaller stateObserverInstaller)
                {
                    var stateObserver = stateObserverInstaller.RegisterStateObserver();
                    container.AddStateObserver(stateObserver);
                }
                else
                {
                    Debug.LogError($"monoBehaviour {monoBehaviour.name} is not of valid type");
                    return;
                }
            }

            foreach (var monoBehaviour in _stateObserverInstallers)
            {
                if (monoBehaviour is IInstaller stateObserverInstaller)
                {
                    stateObserverInstaller.Register(container);
                }
                else
                {
                    Debug.LogError($"monoBehaviour {monoBehaviour.name} is not of valid type");
                    return;
                }
            }
        }
    }
}