using AngleSharp;
using Runic.Framework.Attributes;
using Runic.Framework.Clients;
using System;
using System.Threading.Tasks;
using WikipediaFunctions.Runes;

namespace WikipediaFunctions
{
    public class ArticleFunctions
    {
        private IConfiguration _config { get; set; }
        public static IStatsClient StatsClient { get; set; }
        public static IRuneClient RuneClient { get; set; }

        [ClassInitialise]
        public void Init()
        {
            _config = Configuration.Default.WithDefaultLoader();
        }

        [Function("OpenArticle")]
        public async Task OpenArticle(string article = "")
        {
            var document = await BrowsingContext.New(_config).OpenAsync($"https://en.wikipedia.org/wiki/{article}");
            await RuneClient.SendRunes(new ArticleRune()
            {
                ResponseHtml = document.Body.TextContent,
                CreatedTime = DateTime.Now,
                Name = "ArticleRune"
            });
            StatsClient.CountFunctionSuccess("OpenArticle");
        }
    }
}
