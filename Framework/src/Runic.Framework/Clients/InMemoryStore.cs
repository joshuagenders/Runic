using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            lock (Runes)
            {
                var runes = Runes.Where(r => r.Name == query.RuneName).ToList();
                List<Rune> filteredRunes = new List<Rune>();
                if (query.RequiredProperties.Any())
                {
                    foreach (var rune in runes)
                    {
                        bool hasProperties = true;
                        foreach (var prop in query.RequiredProperties)
                        {
                            if (!rune.GetType()
                                    .GetProperties(BindingFlags.Public)
                                    .Where(p => p.Name == prop.Key && p.GetValue(rune).ToString() == prop.Value)
                                    .Any())
                            {
                                hasProperties = false;
                                break;
                            }
                        }
                        if (hasProperties)
                            filteredRunes.Add(rune);
                    }
                }
                else
                {
                    filteredRunes = runes;
                }
                query.Result = filteredRunes[new Random().Next(0, filteredRunes.Count - 1)];
            }
            return query;
        }

        public void Store(List<Rune> runes)
        {
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
