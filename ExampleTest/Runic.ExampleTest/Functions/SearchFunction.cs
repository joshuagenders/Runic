using System;
using System.Collections.Generic;
using Runic.Framework.Attributes;
using Runic.ExampleTest.Runes;
using System.Net.Http;
using Runic.Framework.Extensions;

namespace Runic.ExampleTest.Functions
{
    public class SearchFunction
    {
        [Function("Search")]
        [MinesRunes("SearchResults")]
        public async void Search(string searchTerm = null, string baseAddress = "")
        {
            if (searchTerm == null)
                searchTerm = GenerateSearchTerm();

            Func<string> searchAction = () =>
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(baseAddress);
                return httpClient.GetStringAsync($"/wiki/{searchTerm}").GetAwaiter().GetResult();
            };

            var result = await searchAction.TimedExecute("Search");

            await RunicIoC.RuneClient.SendRunes(
                new SearchResultsRune()
                {
                    SearchTerm = searchTerm,
                    Results = result.ExecutionResult
                });
        }

        public string GenerateSearchTerm()
        {
            var searchTerms = new List<string> {"carrots", "peas", "horses", "cats"};
            return searchTerms[new Random().Next(0, searchTerms.Count - 1)];
        }
    }
}