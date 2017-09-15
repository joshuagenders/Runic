using AngleSharp;
using Runic.Agent.ExampleTest.Runes;
using Runic.Agent.Framework.Attributes;
using Runic.Agent.Framework.Clients;
using System;
using System.Threading.Tasks;

namespace Runic.Agent.ExampleTest.Functions
{
    public class ArticleFunctions
    {
        private IConfiguration _config { get; set; }
        public static IRuneClient RuneClient { get; set; }

        [BeforeEach]
        public void Init()
        {
            _config = Configuration.Default.WithDefaultLoader();
        }

        [Function("StringReturn")]
        public string ReturnAString(string returnString)
        {
            return returnString;
        }

        [BeforeEach]
        public virtual void BeforeEach()
        {
        }

        [Function("Step1")]
        public virtual void Step1()
        {
        }
        [Function("Step2")]
        public virtual void Step2()
        {
        }
        [Function("Step3")]
        public virtual void Step3()
        {
        }
        [Function("Step4")]
        public virtual void Step4()
        {
        }

        [Function("AsyncWait")]
        public virtual async Task DoWait()
        {
            await Task.CompletedTask;
        }

        [Function("Login")]
        public virtual void DoSomeTask1()
        {
        }

        [Function("Inputs")]
        public virtual void Inputs(string input1, int input2)
        {
        }

        [Function("InputsWithDefault")]
        public virtual void DoSomeTask2(string input1, int input2, string defaultVal = "default")
        {
        }

        [AfterEach]
        public virtual void AfterEach()
        {
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
        }
    }
}
