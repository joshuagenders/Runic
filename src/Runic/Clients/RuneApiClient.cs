using System;
using System.Net.Http;
using Newtonsoft.Json;
using Runic.Data;
using System.Threading.Tasks;
using Runic.Query;
using System.Collections.Generic;

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

        public async Task<HttpResponseMessage> SendRunes(params Rune[] runes)
        {
            return await _client.PostAsync("/api/runes", new StringContent(JsonConvert.SerializeObject(runes)));
        }

        public async Task<List<Rune>> RetrieveRunes(params RuneQuery[] queries)
        {
            var response = await _client.PostAsync("/api/query", new StringContent(JsonConvert.SerializeObject(queries)));
            return JsonConvert.DeserializeObject<List<Rune>>(response.Content.ToString());
        }
    }

    public class RuneDbApiConfig
    {
        public double BackoffFactor { get; set; }
        public string BaseUri { get; set; }
    }
}
