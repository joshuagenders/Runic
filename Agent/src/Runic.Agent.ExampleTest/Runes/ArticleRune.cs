using Runic.Framework.Models;

namespace Runic.Agent.ExampleTest.Runes
{
    public class ArticleRune : Rune
    {
        public ArticleRune()
        {
            Name = "ArticleRune";
        }
        public string ResponseHtml { get; set; }
    }
}
