using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.TestUtility.Runes;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.UnitTests
{
    [TestClass]
    public class InMemoryRuneClientTests
    {
        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenStoringAndGettting_ReturnsRune()
        {
            var runeClient = new InMemoryRuneClient();
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.GetRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenStoringAndTaking_TakesRune()
        {
            var runeClient = new InMemoryRuneClient();
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.TakeRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
        }

        [TestCategory("UnitTest")]
        [TestMethod]
        public async Task WhenStoreAndGetMultiple_ReturnsError()
        {
            var runeClient = new InMemoryRuneClient();
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.TakeRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            var rune2 = await runeClient.TakeRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
            rune2.Result.Should().BeNull();
        }
    }
}
