using System;
using System.Collections.Generic;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab.GameBusHere
{
    public class GameBus
    {
        private readonly ApplicationLogger _logger;
        private readonly List<ISubscriber> _subscribers = new List<ISubscriber>();

        interface ISubscriber
        {
            bool Publish(IGameMessage message);
        }

        class Subscriber<TMessage> : ISubscriber where TMessage : class
        {
            readonly Action<TMessage> _action;
            public Subscriber(Action<TMessage> action)
            {
                _action = action;
            }

            public bool Publish(IGameMessage message)
            {
                if (message is TMessage)
                {
                    _action(message as TMessage);
                    return true;
                }
                return false;
            }
        }

        public GameBus(ApplicationLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Subscriber to a specific message type. The action will be called each time the a message of the specific type is received.
        /// </summary>
        public void Subscribe<TMessage>(Action<TMessage> action) where TMessage : class, IGameMessage
        {
            _subscribers.Add(new Subscriber<TMessage>(action));
        }

        /// <summary>
        /// Publish a message, all subscribers listening for that message type will receive it.
        /// </summary>
        public void Publish<TMessage>(TMessage message) where TMessage : class, IGameMessage
        {
            _logger.LogDebug($"Got message: {message.GetType().Name}, sending...");
            bool anyoneReceived = false;
            foreach (ISubscriber subscriber in _subscribers)
            {
                if (subscriber.Publish(message))
                {
                    anyoneReceived = true;
                }
            }
            _logger.LogDebug($"Message {message.GetType().Name} was received by: {anyoneReceived}");
        }
    }
}
