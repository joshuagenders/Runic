using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyNetQ;
using NLog;
using Runic.Configuration;
using Runic.Core.Models;

namespace Runic.Clients
{
    public class RabbitMessageClient : IRuneClient
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IBus _bus { get; }
        private string _subscriberId { get; }

        public RabbitMessageClient()
        {
            _bus = RabbitHutch.CreateBus(RunicConfiguration.ClientConnectionConfiguration);
            _subscriberId = Guid.NewGuid().ToString("n");
            _logger.Info($"SubscriberId: {_subscriberId}");
        }

        public Task<RuneQuery> RetrieveRunes(RuneQuery queries)
        {
            _logger.Info($"SubscriberId: {_subscriberId}");
            var request = new RuneQueryRequest()
            {
                RuneQuery = queries,
                Id = Guid.NewGuid().ToString("n")
            };
            var task = _bus.RequestAsync<RuneQueryRequest, RuneQueryResponse>(request);
            var result = task.ContinueWith(response => response.GetAwaiter().GetResult().RuneQueryResult);
            _logger.Info(
                new
                {
                    Message = "query_result",
                    SubscriberId = _subscriberId,
                    Result = result
                });
            return result;
        }

        public void SendRunes<T>(params T[] runes)
        {
            _logger.Info($"SubscriberId: {_subscriberId}");
            var request = new RuneStorageRequest<T>
            {
                Runes = new List<T>(runes)
            };
            _bus.Publish(request);
        }
    }
}