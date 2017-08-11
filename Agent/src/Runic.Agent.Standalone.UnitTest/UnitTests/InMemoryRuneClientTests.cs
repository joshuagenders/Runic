using FluentAssertions;
using Microsoft.Extensions.Logging;
using Runic.Agent.Standalone.Clients;
using Runic.Agent.Standalone.Test.TestUtility;
using Runic.Framework.Models;
using System.Threading.Tasks;
using Xunit;

namespace Runic.Agent.Standalone.Test.UnitTests
{
    public class InMemoryRuneClientTests
    {
        private readonly ILoggerFactory _loggerFactory;
        public InMemoryRuneClientTests()
        {
            _loggerFactory = new LoggerFactory();
        }

        [Fact]
        public async Task StoreAndGet_ReturnsRune()
        {

            var runeClient = new InMemoryRuneClient(_loggerFactory);
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.GetRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
        }

        [Fact]
        public async Task StoreAndTake_TakesRune()
        {
            var runeClient = new InMemoryRuneClient(_loggerFactory);
            await runeClient.SendRunes(new FakeRune() { Data = "test" });
            var rune = await runeClient.TakeRunes(new RuneQuery()
            {
                RuneName = "FakeRune",

            });
            (rune.Result as FakeRune).Data.Should().Be("test");
        }

        [Fact]
        public async Task StoreAndGetMultiple_ReturnsError()
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
