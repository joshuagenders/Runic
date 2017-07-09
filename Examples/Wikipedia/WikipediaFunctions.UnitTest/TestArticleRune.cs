using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runic.Framework.Clients;
using Moq;
using System.Threading.Tasks;
using Runic.Framework.Models;

namespace WikipediaFunctions.UnitTest
{
    [TestClass]
    public class TestArticleRune
    {
        private Mock<IRuneClient> _runeClient;
        private Mock<IStatsClient> _statsClient;

        [TestInitialize]
        public void Init()
        {
            _runeClient = new Mock<IRuneClient>();
            _statsClient = new Mock<IStatsClient>();
        }

        [TestMethod]
        public async Task WhenArticleIsLoaded_ThenRuneIsRecorded()
        {
            ArticleFunctions.RuneClient = _runeClient.Object;
            ArticleFunctions.StatsClient = _statsClient.Object;
            var af = new ArticleFunctions();
            af.Init();
            await af.OpenArticle("Valve_Corporation");
            _runeClient.Verify(r => r.SendRunes(It.IsAny<Rune>()));
            _statsClient.Verify(s => s.CountFunctionSuccess("OpenArticle"));
        }
    }
}
