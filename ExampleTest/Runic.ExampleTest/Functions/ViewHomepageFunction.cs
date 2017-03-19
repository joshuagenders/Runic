using Runic.ExampleTest.Runes;
using Runic.Framework.Attributes;
using Runic.Framework.Extensions;
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
            Func<string> openAction = () =>
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(Constants.BaseAddress);
                return httpClient.GetStringAsync("/").GetAwaiter().GetResult();
            };

            var result = await openAction.TimedExecute("OpenHomepage");

            await RunicIoC.RuneClient.SendRunes(
                new HomepageRune()
                {
                    ResponseHtml = result.ExecutionResult
                });
        }
    }
}
