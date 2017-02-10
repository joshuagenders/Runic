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
            //would normally deserialize, but to simplify just cast
            if (itemId == null)
            {
                var rune = (SearchResult)Runes.Retrieve("SearchResults");
                if (rune.itemIds.Count == 0)
                    throw new InvalidRuneException("No item ids in search results");
                itemId = rune.itemIds[new Random().Next(0, rune.itemIds.Count - 1)];
            }

            await new TimedAction("Login", () => OpenItem(itemId)).Execute();
        }
        public string OpenItem(string itemId)
        {
            return "";
        }
    }

    internal class SearchResult
    {
        public List<string> itemIds { get; set; }
    }
}
