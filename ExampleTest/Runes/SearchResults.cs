using System.Collections.Generic;
using Runic.Core.Attributes;
using Runic.Core.Models;

namespace Runic.SystemTest.Runes
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