using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Runic.Agent.Worker.Messaging;
using Runic.Framework.Models;

namespace Runic.Agent.Worker.UnitTest.TestUtility
{
    public class InMemoryMessagingService : IMessagingService
    {
        private Dictionary<Type, Action<object>> _handlers { get; set; }

        public InMemoryMessagingService()
        {
            _handlers = new Dictionary<Type, Action<object>>();
        }
        public void RegisterHandler<T>(Func<T, Task> handler) where T : class
        {
            _handlers[typeof(T)] = async (_) => 
            {
                await handler(Convert.ChangeType(_,typeof(T)) as T);
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

        public void RegisterMessageHandler(Func<SetThreadLevelRequest, Task> handler) => RegisterHandler(handler);
        public void RegisterMessageHandler(Func<AddUpdateFlowRequest, Task> handler) => RegisterHandler(handler);
        public void RegisterMessageHandler(Func<ConstantFlowExecutionRequest, Task> handler) => RegisterHandler(handler);
        public void RegisterMessageHandler(Func<GraphFlowExecutionRequest, Task> handler) => RegisterHandler(handler);
        public void RegisterMessageHandler(Func<GradualFlowExecutionRequest, Task> handler) => RegisterHandler(handler);
        
    }
}
