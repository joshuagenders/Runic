using System.Collections.Generic;
using System.Threading.Tasks;
using Runic.Core.Models;
using EasyNetQ;

namespace Runic.Clients
{
    public class RuneMessageClient : IRuneClient
    {
        public string SubscriberId { get; set; }
        public List<RuneQueryResponse> QueryResponses { get; set; }
        public IBus Bus { get; set; }

        public Task<RuneQuery> RetrieveRunes(RuneQuery queries)
        {
            var request = new RuneQueryRequest();
            var task = Bus.RequestAsync<RuneQueryRequest, RuneQueryResponse>(request);
            return task.ContinueWith(response => {
                return response.Result.RuneQueryResult;
            });
        }

        public void SendRunes<T>(params T[] runes)
        {
            var request = new RuneStorageRequest<T>()
            {
                Runes = new List<T>(runes)
            };
            Bus.Publish(request);
        }
    }
}
