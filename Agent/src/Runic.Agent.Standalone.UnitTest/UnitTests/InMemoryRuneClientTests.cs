using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone.Test.UnitTests
{
    [TestClass]
    public class InMemoryRuneClientTests
    {
        private readonly ILoggerFactory _loggerFactory;
        public InMemoryRuneClientTests()
        {
            _loggerFactory = new LoggerFactory();
        }

        [TestMethod]
        public async Task WhenStoringAndGettting_ReturnsRune()
        {

            var runeClient = new InMemoryRuneClient(_loggerFactory);
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.GetRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
        }

        [TestMethod]
        public async Task WhenStoringAndTaking_TakesRune()
        {
            var runeClient = new InMemoryRuneClient(_loggerFactory);
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.TakeRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
        }

        [TestMethod]
        public async Task WhenStoreAndGetMultiple_ReturnsError()
        {
            var runeClient = new InMemoryRuneClient(_loggerFactory);
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
