using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using RawRabbit;
using Runic.Framework.Models;

namespace Runic.Framework.Clients
{
    public class RabbitMessageClient : IRuneClient
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IBusClient _bus { get; }
        
        public Task<RuneQuery> RetrieveRunes(RuneQuery queries)
        {
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
                    Result = result
                });
            return result;
        }

        public void SendRunes<T>(params T[] runes) where T : Rune
        {
            var request = new RuneStorageRequest<T>
            {
                Runes = new List<T>(runes)
            };

            _bus.PublishAsync(request);
        }
    }
}