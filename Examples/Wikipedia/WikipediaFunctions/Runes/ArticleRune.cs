using Runic.Framework.Models;

namespace WikipediaFunctions.Runes
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
