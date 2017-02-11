using Newtonsoft.Json;
using Runic.Attributes;
using Runic.Data;
using Runic.Exceptions;
using Runic.Orchestration;
using System;
using System.Collections.Generic;

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
                var rune = await Runes.Retrieve("SearchResults");
                var results = JsonConvert.DeserializeObject<SearchResult>(rune.Detail.ToString());
                if (results.itemIds.Count == 0)
                    throw new InvalidRuneException("No item ids in search results");
                itemId = results.itemIds[new Random().Next(0, results.itemIds.Count - 1)];
            }

            await new TimedAction("Login", () => OpenItem(itemId)).Execute();
        }
        public string OpenItem(string itemId)
        {
            return "";
        }
    }

    public class SearchResult
    {
        public List<string> itemIds { get; set; }
    }
}
