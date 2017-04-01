using System;
using System.Threading.Tasks;
using NLog;
using RawRabbit;
using RawRabbit.Context;
using Runic.Framework.Models;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using System.Threading;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private IBusClient _bus { get; }
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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

        public void Subscribe<T>(Func<T, MessageContext, Task> handler)
        {
            try
            {
                _bus.SubscribeAsync(handler);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler)
        {
            Subscribe(handler);   
        }

        public void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler)
        {
            Subscribe(handler);
        }

        public void RegisterConstantFlowHandler(Func<ConstantFlowExecutionRequest, MessageContext, Task> handler)
        {
            Subscribe(handler);
        }

        public void RegisterGraphFlowHandler(Func<GraphFlowExecutionRequest, MessageContext, Task> handler)
        {
            Subscribe(handler);
        }

        public void RegisterGradualFlowHandler(Func<GradualFlowExecutionRequest, MessageContext, Task> handler)
        {
            Subscribe(handler);
        }

        public async Task Run(CancellationToken ct)
        {
            await Task.Run(() =>
            {
                var mre = new ManualResetEventSlim(false);
                ct.Register(() => mre.Set());
                mre.Wait();
            }, ct);
            await _bus.ShutdownAsync();
        }
    }
}
