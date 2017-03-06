using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Runic.Framework.Clients;
using Runic.Core.Attributes;
using Runic.ExampleTest.Runes;

namespace Runic.ExampleTest.Functions
{
    public class SearchFunction
    {
        [Function("Search")]
        [MinesRunes("SearchResults")]
        [MutableParameter("searchTerm", typeof(string))]
        [MutableParameter("searchType", typeof(string))]
        public async void Search(string searchTerm = null, string searchType = null)
        {
            if (searchTerm == null)
                if (searchType == null)
                    searchTerm = GenerateSearchTerm();
                else
                    searchTerm = GenerateSearchTerm(searchType);
            SearchResults exampleResponse = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://myexample.com");
                    var response = await client.GetAsync($"/search?q={searchTerm}");
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    exampleResponse = JsonConvert.DeserializeObject<SearchResults>(stringResponse);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }

            new RabbitMessageClient().SendRunes(exampleResponse);
        }

        public string GenerateSearchTerm()
        {
            var searchTerms = new List<string> {"carrots", "peas", "horses", "cats"};
            return searchTerms[new Random().Next(0, searchTerms.Count - 1)];
        }

        public string GenerateSearchTerm(string searchType)
        {
            var type1 = new List<string> {"carrots", "peas"};
            var type2 = new List<string> {"horses", "cats"};
            if (searchType == "type1")
                return type1[new Random().Next(0, type1.Count - 1)];
            return type2[new Random().Next(0, type2.Count - 1)];
        }
    }
}