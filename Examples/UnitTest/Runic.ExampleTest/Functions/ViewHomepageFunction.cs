using Runic.ExampleTest.Runes;
using Runic.Framework.Attributes;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runic.ExampleTest.Functions
{
    public class ViewHomepageFunction
    {
        [Function("OpenHomepage")]
        public async Task Open()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Constants.BaseAddress);
            var result = httpClient.GetStringAsync("/").GetAwaiter().GetResult();

            await RunicIoC.RuneClient.SendRunes(
                new HomepageRune()
                {
                    ResponseHtml = result
                });
        }
    }
}
