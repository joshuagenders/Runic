using System.Collections.Generic;
using System.Linq;
using Runic.Framework;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using Runic.ExampleTest.Runes;
using Runic.Framework.Orchestration;

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
            var queryResults = await RunicIoC.RuneClient.RetrieveRunes(runeQuery);
            var results = queryResults.ToResultsList();
            var user = results.First(r => r.Name == typeof(AuthenticatedUser).Name) as AuthenticatedUser;
            var customerId = user?.Username;
            var basket = results.First(r => r.Name == typeof(BasketCreated).Name) as BasketCreated;
            var basketId = basket?.BasketId;

            await new TimedAction("PlaceOrder", () => DoPlaceOrder(basketId, customerId)).Execute();
        }

        public async void DoPlaceOrder(string basketId, string userId)
        {
            
        }
    }
}