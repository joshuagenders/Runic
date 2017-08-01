using Runic.ExampleTest.Runes;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Runic.ExampleTest.Functions
{
    public class OpenFirstLinkFunction
    {
        [Function("OpenFirstLink")]
        public async Task Open()
        {
            // retrieve rune
            var query = new RuneQuery() { RuneName = "SearchResult" };
            var runes = await RunicIoC.RuneClient.RetrieveRunes(query);
            SearchResultsRune rune = (SearchResultsRune)runes.Result;
            
            // extract link
            var regex = new Regex("<a href=\"/wiki/(.*)\"");
            var matchResults = regex.Match(rune.Results);
            if (!matchResults.Success)
                throw new Exception("No first link found");
            var firstLink = matchResults.Value;

            // get next page
            
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Constants.BaseAddress);
            var result = httpClient.GetStringAsync($"/{firstLink}").GetAwaiter().GetResult();
            
            // store rune
            await RunicIoC.RuneClient.SendRunes(
                new SearchResultsRune()
                {
                    SearchTerm = firstLink,
                    Results = result
                });
        }
    }
}
