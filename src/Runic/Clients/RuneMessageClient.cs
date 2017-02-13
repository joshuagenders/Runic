using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runic.Core;
using EasyNetQ;

namespace Runic.Clients
{
    public class RuneMessageClient : IRuneClient
    {
        public string SubscriberId { get; set; }
        public List<RuneQueryResponse> QueryResponses { get; set; }
        public IBus Bus { get; set; }

        public Task<List<Rune>> RetrieveRunes(params RuneQuery[] queries)
        {
            var request = new RuneQueryRequest();
            var task = Bus.RequestAsync<RuneQueryRequest, RuneQueryResponse>(request);
            return task.ContinueWith(response => {
                return response.Result.Runes;
            });
        }

        public void SendRunes(params Rune[] runes)
        {
            var request = new RuneStorageRequest()
            {
                Runes = new List<Rune>(runes)
            };
            Bus.Publish(request);
        }
    }
}
