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
        
        public async Task<RuneQuery> RetrieveRunes(RuneQuery queries)
        {
            var request = new RuneQueryRequest()
            {
                RuneQuery = queries,
                Id = Guid.NewGuid().ToString("n")
            };

            var result = await _bus.RequestAsync<RuneQueryRequest, RuneQueryResponse>(request);

            _logger.Info(
                new
                {
                    Message = "query_result",
                    Result = result.RuneQueryResult
                });

            return result.RuneQueryResult;
        }

        public async Task SendRunes<T>(params T[] runes) where T : Rune
        {
            var request = new RuneStorageRequest<T>
            {
                Runes = new List<T>(runes)
            };

            await _bus.PublishAsync(request);
        }
    }
}