using System.Collections.Generic;
using Runic.Framework.Attributes;
using Runic.Framework.Models;

namespace Runic.ExampleTest.Runes
{
    public class SearchResultsRune : Rune
    {
        [IndexedProperty]
        public string SearchTerm { get; set; }

        public string Results { get; set; }
    }
}