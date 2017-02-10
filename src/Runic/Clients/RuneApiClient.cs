using System;
using System.Net.Http;
using Newtonsoft.Json;
using Runic.Data;
using System.Threading.Tasks;
using Runic.Query;
using System.Text;
using System.Text.Encodings.Web;

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
        //async
        public async Task<HttpResponseMessage> SendRune(Rune rune)
        {
            return await _client.PostAsync("/runes", new StringContent(JsonConvert.SerializeObject(rune)));
        }

        public async Task<HttpResponseMessage> RetrieveRune(RuneQuery query)
        {
            var encoder = new HtmlEncoder();
            return await _client.GetAsync("/runes", .new StringContent(JsonConvert.SerializeObject(rune)));
        }
    }

    public class RuneDbApiConfig
    {
        public double BackoffFactor { get; set; }
        public string BaseUri { get; set; }
    }
}
