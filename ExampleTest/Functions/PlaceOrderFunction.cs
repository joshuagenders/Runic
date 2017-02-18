using Newtonsoft.Json;
using Runic.Core.Attributes;
using Runic.Data;
using Runic.Orchestration;
using Runic.Core.Messaging;
using Runic.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Runic.SystemTest.Runes;
using System.Linq;

namespace Runic.ExampleTest.Functions
{
    public class PlaceOrderFunction
    {
        [Function("PlaceOrder")]
        [RequiresLinkedRunes("CustomerId", "AuthenticatedUser", "BasketCreated")]
        public async void PlaceOrder()
        {
            var runeQuery = new RuneQuery()
            {
                RuneName = "AuthenticatedUser",
                RequiredProperties =
                    new Dictionary<string, string>() {
                        { "CustomerName", "$fakeuser.*" }
                    },
                RequiredLinks =
                    new List<RuneQuery>() {
                        new RuneQuery()
                        {
                            RuneName = "BasketCreated",
                            RequiredProperties = new Dictionary<string, string>()
                            {
                                { "Active", "true" }
                            }
                        }
                    }
            };
            var queryResults = await Runes.Retrieve(runeQuery);
            var results = queryResults.ToResultsList();
            var user = results.Where(r => r.Name == typeof(AuthenticatedUser).Name).First() as AuthenticatedUser;
            var customerId = user.Username;
            var basket = results.Where(r => r.Name == typeof(BasketCreated).Name).First() as BasketCreated;
            var basketId = basket.BasketId;

            await new TimedAction("PlaceOrder", () => DoPlaceOrder(basketId, customerId)).Execute();
        }

        public async void DoPlaceOrder(string basketId, string userId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var postBody = JsonConvert.SerializeObject(new { basketId = basketId, userId = userId });
                    client.BaseAddress = new Uri("http://myexample.com");
                    var response = await client.PostAsync($"/order", new StringContent(postBody));
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }
    }
}
