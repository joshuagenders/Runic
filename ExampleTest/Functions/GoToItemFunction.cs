using System;
using Runic.Clients;
using Runic.Core.Attributes;
using Runic.Core.Models;
using Runic.Exceptions;
using Runic.Orchestration;
using Runic.ExampleTest.Runes;

namespace Runic.ExampleTest.Functions
{
    public class GoToItemFunction
    {
        [Function("GoToItem")]
        [RequiresRunes("SearchResults")]
        [MutableParameter("itemId", typeof(string))]
        public async void GoToItem(string itemId = null)
        {
            if (itemId == null)
            {
                var queryResults = await new RuneMessageClient().RetrieveRunes(new RuneQuery());
                var searchResults = queryResults.Result as SearchResults;
                if (searchResults == null || searchResults.Results.Count == 0)
                    throw new InvalidRuneException("No item ids in search results");
                itemId = searchResults.Results[new Random().Next(0, searchResults.Results.Count - 1)];
            }

            await new TimedAction("Login", () => OpenItem(itemId)).Execute();
        }

        public string OpenItem(string itemId)
        {
            return "";
        }
    }
}