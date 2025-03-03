using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.EventBusSystem
{
    internal class EventBusHelper
    {
        private readonly Dictionary<Type, List<Type>> s_CashedSubscriberTypes = new();

        public List<Type> GetSubscriberTypes(
            IGlobalSubscriber globalSubscriber)
        {
            Type type = globalSubscriber.GetType();
            if (s_CashedSubscriberTypes.TryGetValue(type, out List<Type> types))
            {
                return types;
            }

            List<Type> subscriberTypes = type
                .GetInterfaces()
                .Where(t => t.GetInterfaces()
                    .Contains(typeof(IGlobalSubscriber)))
                .ToList();

            s_CashedSubscriberTypes[type] = subscriberTypes;
            return subscriberTypes;
        }
    }
}