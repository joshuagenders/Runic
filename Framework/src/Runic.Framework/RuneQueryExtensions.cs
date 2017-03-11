using System.Collections.Generic;
using Runic.Framework.Models;

namespace Runic.Framework
{
    public static class RuneQueryExtensions
    {
        public static List<Rune> ToResultsList(this RuneQuery query)
        {
            var runes = new List<Rune>();
            return TraverseTree(runes, query);
        }

        public static List<Rune> TraverseTree(List<Rune> runes, RuneQuery runeQuery)
        {
            //base case already visited
            if (runes.Contains(runeQuery.Result))
                return runes;

            runes.Add(runeQuery.Result);
            foreach (var query in runeQuery.RequiredLinks)
                if (!runes.Contains(query.Result))
                    runes = TraverseTree(runes, query);

            return runes;
        }
    }
}