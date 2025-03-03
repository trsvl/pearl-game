using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.EventBusSystem
{
    public class EventBus
    {
        private readonly Dictionary<Type, SubscribersList<IGlobalSubscriber>> globalSubscribers = new();
        private readonly EventBusHelper helper = new();

        public void Subscribe(IGlobalSubscriber subscriber)
        {
            List<Type> subscriberTypes = helper.GetSubscriberTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (!globalSubscribers.ContainsKey(t))
                {
                    globalSubscribers[t] = new SubscribersList<IGlobalSubscriber>();
                }

                globalSubscribers[t].Add(subscriber);
            }
        }

        public void Unsubscribe(IGlobalSubscriber subscriber)
        {
            List<Type> subscriberTypes = helper.GetSubscriberTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (globalSubscribers.ContainsKey(t))
                    globalSubscribers[t].Remove(subscriber);
            }
        }

        public void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
            where TSubscriber : class, IGlobalSubscriber
        {
            SubscribersList<IGlobalSubscriber> subscribers = globalSubscribers[typeof(TSubscriber)];

            subscribers.Executing = true;
            foreach (IGlobalSubscriber subscriber in subscribers.List)
            {
                try
                {
                    action.Invoke(subscriber as TSubscriber);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            subscribers.Executing = false;
            subscribers.Cleanup();
        }
    }
}