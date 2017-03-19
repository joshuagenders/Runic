using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Framework.Clients
{
    public class InMemoryStore
    {
        private List<Rune> Runes { get; set; }
        public int MaxObjects { get; set; }

        public InMemoryStore()
        {
            Runes = new List<Rune>();
            MaxObjects = 5000; //todo config
        }

        public RuneQuery Retrieve(RuneQuery query)
        {
            lock (Runes) {
                var runes = Runes.Where(r => r.Name == query.RuneName).ToList();
                List<Rune> filteredRunes = new List<Rune>();
                if (query.RequiredProperties.Any())
                {
                    //todo filter on properties
                    filteredRunes = runes;
                }
                else
                {
                    filteredRunes = runes;
                }

                if (!filteredRunes.Any())
                    return query;

                query.Result = filteredRunes[new Random().Next(0, filteredRunes.Count - 1)];
            }
            return query;
        }

        public void Store(List<Rune> runes)
        {
            if (runes == null || !runes.Any())
                return;

            lock (Runes) {
                Runes.AddRange(runes);
                if (runes.Count > MaxObjects)
                {
                    runes.Where(r => r.RuneExpiry < DateTime.Now)
                         .ToList()
                         .ForEach(r => Runes.Remove(r));
                }
            }
            //todo remove non expired
        }
    }
}
