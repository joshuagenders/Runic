using Runic.Agent.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class InMemoryMessagingService : IMessagingService
    {
        private Dictionary<Type, Action<object>> _handlers { get; set; }

        public InMemoryMessagingService()
        {
            _handlers = new Dictionary<Type, Action<object>>();
        }
        public void RegisterMessageHandler<T>(Action<T> handler) where T : class
        {
            _handlers[typeof(T)] = (_) => 
            {
                handler(Convert.ChangeType(_,typeof(T)) as T);
            };
        }

        public void PublishMessage<T>(T message)
        {
            foreach (var handler in _handlers)
            {
                Action<object> handle;
                if (_handlers.TryGetValue(typeof(T), out handle))
                {
                    handle(message);
                }
                else if (typeof(T).Equals(handler.Key.GetType()) || typeof(T).IsAssignableFrom(handler.Key.GetType()))
                {
                    handler.Value(message);
                }
            }
        }

        public Task RunServiceAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
