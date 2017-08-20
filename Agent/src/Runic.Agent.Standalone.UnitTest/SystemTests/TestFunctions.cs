﻿using Runic.Framework.Attributes;

namespace Runic.Agent.Standalone.Test.SystemTests
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
    }
}