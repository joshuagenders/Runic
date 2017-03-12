using System.Collections.Generic;
using Runic.Framework.Attributes;
using Runic.Framework.Models;

namespace Runic.ExampleTest.Runes
{
    public class SearchResults : Rune
    {
        public SearchResults() : base("AuthenticatedUser")
        {
        }

        [IndexedProperty]
        public string Postcode { get; set; }

        public List<string> Results { get; set; }
    }
}