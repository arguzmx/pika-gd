using PIKA.Infrastructure.EventBus.Events;
using System;

namespace PIKA.Infrastructure.EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);


        public void Subscribe(Type T, Type TH);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
