using Runic.Framework.Attributes;

namespace Runic.Agent.Standalone.Test.FunctionalTests
{
    public class TestFunctions
    {
        [Function("Wikipedia")]
        public void GetWikipediaArticle()
        {

        }

        [Function("WikipediaWithInput")]
        public void GetWikipediaArticle(string article)
        {

        }

        [Function("StringReturn")]
        public string StringReturn(string returnValue)
        {
            return returnValue;
        }

        [Function("Step1")]
        public string Step1(string returnValue)
        {
            return returnValue;
        }

        [Function("Step2")]
        public string Step2(string returnValue)
        {
            return returnValue;
        }

        [Function("Step3")]
        public string Step3(string returnValue)
        {
            return returnValue;
        }
    }
}
