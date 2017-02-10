using Newtonsoft.Json;
using Runic.Attributes;
using Runic.Data;
using Runic.Orchestration;
using Runic.Query;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Runic.SystemTest
{
    public class PlaceOrderFunction
    {
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
                    LinkedProperties = new string[] { "CustomerId" }
                },
                new RuneQuery()
                {
                    RuneName = "BasketCreated",
                    EnableRegex = false,
                    LinkedProperties = new string[] { "Body.CustomerId" }
                });
            
            var user = JsonConvert.DeserializeObject<ExampleResponse>((string)runes[0]);
            var customerId = user.CustomerId;
            var basket = JsonConvert.DeserializeObject<ExampleResponse>((string)runes[1]);
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
