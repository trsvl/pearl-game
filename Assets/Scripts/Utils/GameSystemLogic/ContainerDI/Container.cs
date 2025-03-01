using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.GameSystemLogic.ContainerDI
{
    public class Container
    {
        private readonly Dictionary<Type, List<object>> _services = new();
        private readonly Dictionary<Type, object> _singletonServices = new();
        private readonly List<IStateObserver> _stateObservers = new();


        public void AddStateObserver(IStateObserver stateObserver)
        {
            if (stateObserver is null)
            {
                Debug.LogError($"{nameof(stateObserver)} is null");
                return;
            }

            _stateObservers.Add(stateObserver);
        }

        public void Bind<T>(T service, bool isSingleton = false)
        {
            if (service == null)
            {
                Debug.LogError("Service is null");
                return;
            }

            var type = typeof(T);

            if (_singletonServices.ContainsKey(type))
            {
                Debug.LogError("Service already registered as singleton: " + type);
                return;
            }

            if (isSingleton)
            {
                if (_services.ContainsKey(type))
                {
                    Debug.LogError("Service already registered as default service: " + type);
                    return;
                }

                _singletonServices.Add(type, service);
            }
            else
            {
                if (!_services.ContainsKey(type))
                {
                    _services.Add(type, new List<object>());
                }

                _services[type].Add(service);
            }

            foreach (var stateObserver in _stateObservers)
            {
                stateObserver.AddListener(service);
            }
        }

        public void BindListener<T>(T service)
        {
            foreach (var stateObserver in _stateObservers)
            {
                stateObserver.AddListener(service);
            }
        }
        
        public void UnbindListener<T>(T service)
        {
            foreach (var stateObserver in _stateObservers)
            {
                stateObserver.RemoveListener(service);
            }
        }

        public T GetService<T>()
        {
            var type = typeof(T);

            if (_singletonServices.TryGetValue(type, out object singletonService))
            {
                return (T)singletonService;
            }

            if (_services.TryGetValue(type, out List<object> service))
            {
                if (service.Count > 1)
                {
                    Debug.LogError("Multiple services found. Type: " + type);
                    return default;
                }

                if (service.Count == 1)
                {
                    return (T)service[0];
                }
            }

            Debug.LogError("No services found. Type: " + type);
            return default;
        }

        public IEnumerable<T> GetAllServices<T>()
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var services))
            {
                return (IEnumerable<T>)services;
            }

            Debug.LogError("No services found. Type: " + type);
            return default;
        }
    }
}