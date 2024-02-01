﻿using System;
using System.Collections.Generic;

namespace Api.Application.Events
{
	public interface IEventBusSubscriptionManager
	{
		event EventHandler<string> OnEventRemoved;

		bool IsEmpty { get; }
		bool HasSubscriptionsForEvent(string eventName);

		string GetEventIdentifier<TEvent>();
		Type GetEventTypeByName(string eventName);
		IEnumerable<Subscription> GetHandlersForEvent(string eventName);
		Dictionary<string, List<Subscription>> GetAllSubscriptions();

		void AddSubscription<TEvent, TEventHandler>()
			where TEvent : IntegrationEvent
			where TEventHandler : IIntegrationEventHandler<TEvent>;

		void RemoveSubscription<TEvent, TEventHandler>()
			where TEvent : IntegrationEvent
			where TEventHandler : IIntegrationEventHandler<TEvent>;

		void Clear();
	}
}