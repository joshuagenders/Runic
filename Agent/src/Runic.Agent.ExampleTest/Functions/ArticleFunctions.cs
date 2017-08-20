using AngleSharp;
using Runic.Agent.ExampleTest.Runes;
using Runic.Framework.Attributes;
using Runic.Framework.Clients;
using System;
using System.Threading.Tasks;

namespace Runic.Agent.ExampleTest.Functions
{
    public class ArticleFunctions
    {
        private IConfiguration _config { get; set; }
        public static IStatsClient StatsClient { get; set; }
        public static IRuneClient RuneClient { get; set; }

        [BeforeEach]
        public void Init()
        {
            _config = Configuration.Default.WithDefaultLoader();
        }

        [Function("OpenArticle")]
        public async Task OpenArticle(string article = "Science_fiction")
        {
            var articleLink = $"https://en.wikipedia.org/wiki/{article}";
            var browsingContext = BrowsingContext.New(_config);
            var document = await browsingContext.OpenNewAsync(articleLink);

            if (RuneClient != null && document.Body != null)
            {
                await RuneClient?.SendRunes(new ArticleRune()
                {
                    ResponseHtml = document.Body.TextContent,
                    CreatedTime = DateTime.Now,
                    Name = "ArticleRune"
                });
            }
            StatsClient?.CountFunctionSuccess("OpenArticle");
        }
    }
}
