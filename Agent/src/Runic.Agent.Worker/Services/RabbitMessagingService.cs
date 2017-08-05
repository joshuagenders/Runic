using Runic.Agent.Worker.Messaging;
using RawRabbit;
using RawRabbit.vNext;
using RawRabbit.Configuration;
using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.Services
{
    public class RabbitMessagingService : IMessagingService
    {
        private IBusClient _bus { get; }

        public RabbitMessagingService()
        {
            var busConfig = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                Hostnames = { "localhost" }
            };
            _bus = BusClientFactory.CreateDefault(busConfig);
        }

        public void RegisterMessageHandler(Func<SetThreadLevelRequest, Task> handler) =>
            _bus.SubscribeAsync<SetThreadLevelRequest>(async (msg, context) => 
                await handler(msg));

        public void RegisterMessageHandler(Func<AddUpdateFlowRequest, Task> handler) =>
            _bus.SubscribeAsync<AddUpdateFlowRequest>(async (msg, context) =>
                await handler(msg));

        public void RegisterMessageHandler(Func<ConstantFlowExecutionRequest, Task> handler) =>
            _bus.SubscribeAsync<ConstantFlowExecutionRequest>(async (msg, context) =>
                await handler(msg));

        public void RegisterMessageHandler(Func<GraphFlowExecutionRequest, Task> handler) =>
            _bus.SubscribeAsync<GraphFlowExecutionRequest>(async (msg, context) =>
                await handler(msg));

        public void RegisterMessageHandler(Func<GradualFlowExecutionRequest, Task> handler) =>
            _bus.SubscribeAsync<GradualFlowExecutionRequest>(async (msg, context) =>
                await handler(msg));

        public void PublishMessage<T>(T message)
        {
            _bus.PublishAsync(message);
        }

        public async Task RunServiceAsync(CancellationToken ctx = default(CancellationToken))
        {
            await Task.Run(() =>
            {
                var mre = new ManualResetEventSlim(false);
                ctx.Register(() => mre.Set());
                mre.Wait();
            }, ctx);
            await _bus.ShutdownAsync();
        }
    }
}
