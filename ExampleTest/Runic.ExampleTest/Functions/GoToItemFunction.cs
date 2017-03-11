﻿using System;
using Runic.Framework.Clients;
using Runic.Core.Attributes;
using Runic.Core.Models;
using Runic.Framework.Orchestration;
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
                var queryResults = await RunicIoC.RuneClient.RetrieveRunes(new RuneQuery());
                var searchResults = queryResults.Result as SearchResults;
                if (searchResults == null || searchResults.Results.Count == 0)
                    throw new InvalidRuneException("No item ids in search results");
                itemId = searchResults.Results[new Random().Next(0, searchResults.Results.Count - 1)];
            }

            await new TimedAction("Login", () => OpenItem(itemId)).Execute();
        }

        private string OpenItem(string itemId)
        {
            return "";
        }
    }
}