using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.UnitTest.Runic
{
    [TestClass]
    public class TestInMemoryClient
    {
        [TestMethod]
        public void TestStoreRetrieveDirectly()
        {
            var store = new InMemoryStore();
            store.Store(
                new List<Rune> {
                    new FakeRune()
                    {
                        Name = "FakeRune",
                        FakeProp = "Bob"
                    }
                });
            var result = store.Retrieve(new RuneQuery()
            {
                RuneName = "FakeRune",
                RequiredProperties = new Dictionary<string, string>()
                {
                    { "FakeProp","Bob" }
                }
            }).Result as FakeRune;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.FakeProp, "Bob");
        }

        [TestMethod]
        public async Task TestStoreAndRetrieve()
        {
            var client = new InMemoryClient();
            await client.SendRunes(new FakeRune()
            {
                Name = "FakeRune",
                FakeProp = "Bob"
            });
        
            var result = await client.RetrieveRunes(new RuneQuery()
            {
                RuneName = "FakeRune",
                RequiredProperties = new Dictionary<string, string>()
                {
                    { "FakeProp","Bob" }
                }
            });
            var res = result.Result as FakeRune;
            Assert.IsNotNull(res);
            Assert.AreEqual(res.FakeProp, "Bob");
        }
    }
    public class FakeRune : Rune
    {
        public string FakeProp { get; set; }
        public int FakeProp2 { get; set; }
    }
}
