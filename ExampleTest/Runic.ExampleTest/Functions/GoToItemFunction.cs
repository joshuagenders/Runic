using System;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
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
            await new TimedAction("Login", () => OpenItem(itemId)).Execute();
        }

        private string OpenItem(string itemId)
        {
            return "";
        }
    }
}