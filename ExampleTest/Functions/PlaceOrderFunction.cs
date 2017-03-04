using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Runic.Clients;
using Runic.Core;
using Runic.Core.Attributes;
using Runic.Core.Models;
using Runic.ExampleTest.Runes;
using Runic.Orchestration;

namespace Runic.ExampleTest.Functions
{
    public class PlaceOrderFunction
    {
        [Function("PlaceOrder")]
        [RequiresLinkedRunes("CustomerId", "AuthenticatedUser", "BasketCreated")]
        public async void PlaceOrder()
        {
            var runeQuery = new RuneQuery
            {
                RuneName = "AuthenticatedUser",
                RequiredProperties =
                    new Dictionary<string, string>
                    {
                        {"CustomerName", "$fakeuser.*"}
                    },
                RequiredLinks =
                    new List<RuneQuery>
                    {
                        new RuneQuery
                        {
                            RuneName = "BasketCreated",
                            RequiredProperties = new Dictionary<string, string>
                            {
                                {"Active", "true"}
                            }
                        }
                    }
            };
            var queryResults = await new RabbitMessageClient().RetrieveRunes(runeQuery);
            var results = queryResults.ToResultsList();
            var user = results.First(r => r.Name == typeof(AuthenticatedUser).Name) as AuthenticatedUser;
            var customerId = user?.Username;
            var basket = results.First(r => r.Name == typeof(BasketCreated).Name) as BasketCreated;
            var basketId = basket?.BasketId;

            await new TimedAction("PlaceOrder", () => DoPlaceOrder(basketId, customerId)).Execute();
        }

        public async void DoPlaceOrder(string basketId, string userId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var postBody = JsonConvert.SerializeObject(new {basketId, userId});
                    client.BaseAddress = new Uri("http://myexample.com");
                    await client.PostAsync("/order", new StringContent(postBody));
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }
    }
}