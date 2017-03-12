using System;
using System.Collections.Generic;
using Runic.Framework.Attributes;
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
                searchTerm = searchType == null ? GenerateSearchTerm() : GenerateSearchTerm(searchType);

            SearchResults exampleResponse = null;

            RunicIoC.RuneClient.SendRunes(exampleResponse);
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