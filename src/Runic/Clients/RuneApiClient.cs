using System;
using System.Net.Http;
using Newtonsoft.Json;
using Runic.Data;
using System.Threading.Tasks;
using Runic.Query;

namespace Runic.Clients
{
    public class RuneApiClient : IRuneClient
    {
        HttpClient _client { get; set; }
        RuneDbApiConfig _config { get; set; }

        public RuneApiClient(RuneDbApiConfig config)
        {
            _client = new HttpClient();
            _config = config;
            _client.BaseAddress = new Uri(_config.BaseUri);
        }

        public async Task<HttpResponseMessage> SendRune(Rune rune)
        {
            return await _client.PostAsync("/runes", new StringContent(JsonConvert.SerializeObject(rune)));
        }

        public async Task<HttpResponseMessage> RetrieveRune(params RuneQuery[] queries)
        {
            return await _client.PostAsync("/query", new StringContent(JsonConvert.SerializeObject(queries)));
        }
    }

    public class RuneDbApiConfig
    {
        public double BackoffFactor { get; set; }
        public string BaseUri { get; set; }
    }
}
