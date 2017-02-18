using Newtonsoft.Json;
using Runic.Core;
using Runic.Core.Attributes;
using Runic.Data;
using Runic.Exceptions;
using Runic.Orchestration;
using Runic.SystemTest.Runes;
using System;

namespace Runic.ExampleTest
{
    public class GoToItemFunction
    {
        [RequiresRunes("SearchResults")]
        [MutableParameter("itemId", typeof(string))]
        public async void GoToItem(string itemId = null)
        {
            if (itemId == null)
            {
                var queryResults = await Runes.Retrieve(new RuneQuery());
                var searchResults = queryResults.Result as SearchResults;
                if (searchResults.Results.Count == 0)
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
