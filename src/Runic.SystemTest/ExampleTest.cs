using Runic.Attributes;
using Runic.Data;
using Runic.Exceptions;
using Runic.Orchestration;
using Runic.Query;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace Runic.SystemTest
{
    public class ExampleTest
    {
        [MinesRunes("AuthenticatedUser")]
        [MutableParameter("userDetails", typeof(UserDetails))]
        public async void Login(UserDetails userDetails = null)
        {
            if (userDetails == null)
            {
                userDetails = GetUserDetails();
            }
            var result = await new TimedAction("Login", () => DoLogin(userDetails)).Execute();
            var response = (ExampleResponse)result.ExecutionResult;
            var rune = new Rune()
            {
                Name = "AuthenticatedUser",
                Detail = result
            };
            rune.IndexedProperties[Ref.Indexes[(int)Ref.Keys.CustomerId]] = response.CustomerId;
            rune.IndexedProperties[Ref.Indexes[(int)Ref.Keys.AuthCookie]] = response.AuthCookie;

            Runes.Mine(rune);
        }

        public ExampleResponse DoLogin(UserDetails userDetails)
        {
            return new ExampleResponse();
        }

        public UserDetails GetUserDetails()
        {
            //get from file, db etc.
            //can pass run id to db so that client reservation works, create datastore function to manage core data
            //you can then set that as an input stream for a flow that you've constructed
            return new UserDetails()
            {
                UserName = "",
                Password = ""
            };
        }

        [MinesRunes("SearchResults")]
        [MutableParameter("searchTerm", typeof(string))]
        public async void Search(string searchTerm = null)
        {
            if (searchTerm == null)
                searchTerm = GenerateSearchTerm();
            ExampleResponse exampleResponse = null;
            string stringResponse = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://myexample.com");
                    var response = await client.GetAsync($"/search?q={searchTerm}");
                    stringResponse = await response.Content.ReadAsStringAsync();
                    exampleResponse = JsonConvert.DeserializeObject<ExampleResponse>(stringResponse);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
            var rune = new Rune()
            {
                Detail = stringResponse,
                Name = "SearchResults",
            };
            rune.IndexedProperties[Ref.Indexes[(int)Ref.Keys.CustomerId]] = exampleResponse.CustomerId;
            rune.IndexedProperties[Ref.Indexes[(int)Ref.Keys.Area]] = exampleResponse.Area;

            Runes.Mine(rune);
        }

        public string GenerateSearchTerm()
        {
            var searchTerms = new List<string>() { "carrots", "peas" };
            return searchTerms[new Random().Next(0, searchTerms.Count - 1)];
        }

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

        [RequiresLinkedRunes("CustomerId", "AuthenticatedUser", "BasketCreated")]
        public async void PlaceOrder()
        {
            var runes = Runes.RetrieveMultiple(
                new RuneQuery()
                {
                    RuneName = "AuthenticatedUser",
                    EnableRegex = true,
                    RequiredProperties = new Dictionary<string, string>() {
                        { "CustomerName", "$fakeuser.*" }
                    },
                    LinkedProperties = new string[]{ "CustomerId" }
                },
                new RuneQuery()
                {
                    RuneName = "BasketCreated",
                    EnableRegex = false,
                    LinkedProperties = new string[] { "Body.CustomerId" }
                });

            //would normally deserialize etc.
            var user = (ExampleResponse)runes[0];
            var customerId = user.CustomerId;
            var basket = (ExampleResponse)runes[1];
            var basketId = basket.BasketId;

            await new TimedAction("PlaceOrder", () => DoPlaceOrder(basketId, customerId)).Execute();
        }
        public string DoPlaceOrder(string basketId, string userId)
        {
            return "";
        }

        public class UserDetails
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
    public class SearchResult
    {
        public List<string> itemIds { get; set; }
    }
}
