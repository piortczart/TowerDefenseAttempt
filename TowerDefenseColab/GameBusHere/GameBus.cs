using System;
using System.Collections.Generic;

namespace TowerDefenseColab
{

    public class GameBus
    {
        interface ISubscriber
        {
            void Publish(IGameMessage message);
        }

        class Subscriber<TMessage> : ISubscriber where TMessage : class
        {
            Action<TMessage> _action;
            public Subscriber(Action<TMessage> action)
            {
                _action = action;
            }

            public void Publish(IGameMessage message)
            {
                if (typeof(TMessage).IsAssignableFrom(message.GetType()))
                {
                    _action(message as TMessage);
                }
            }
        }

        private List<ISubscriber> _subscribers = new List<ISubscriber>();

        public void Subscribe<TMessage>(Action<TMessage> action) where TMessage : class, IGameMessage
        {
            _subscribers.Add(new Subscriber<TMessage>(action));
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class, IGameMessage
        {
            foreach (ISubscriber subscriber in _subscribers)
            {
                subscriber.Publish(message);
            }
        }
    }
}
