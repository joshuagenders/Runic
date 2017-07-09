using System;
using System.Threading.Tasks;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.Worker.Clients
{
    public class InMemoryRuneClient : IRuneClient
    {
        public ConcurrentDictionary<string, ConcurrentBag<Rune>> RuneBags { get; set; }
        public int MaxRuneCount { get; set; }
        public int RuneCount => RuneBags.Sum(q => q.Value.Count);

        public InMemoryRuneClient(int maxRuneCount = 1024)
        {
            RuneBags = new ConcurrentDictionary<string, ConcurrentBag<Rune>>();
            MaxRuneCount = maxRuneCount;
        }

        public async Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            var bag = GetOrCreateBag(query.RuneName);
            query.Result = bag.PeekRandom(r => RuneHasMatchingProperties(r, query.RequiredProperties));
            return await Task.FromResult(query);
        }

        private bool RuneHasMatchingProperties(Rune rune, Dictionary<string, string> properties)
        {
            bool matches = true;
            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.Value))
                {
                    if (!PropertyExists(rune, property.Key))
                    {
                        matches = false;
                        break;
                    }
                }
                else
                {
                    if (!PropertyExists(rune, property.Key, property.Value))
                    {
                        matches = false;
                        break;
                    }
                }
            }
            return matches;
        }

        private bool PropertyExists(Rune rune, string propertyName)
        {
            //todo support x.y.z recursive (non base properties)
            return rune.GetType().GetProperties().Select(p => p.Name).Contains(propertyName);
        }

        private bool PropertyExists(Rune rune, string propertyName, string propertyValue)
        {
            var prop = rune.GetType().GetProperties().Where(p => p.Name == propertyName);
            if (prop.Count() > 0)
            {
                if (prop.FirstOrDefault().GetValue(rune).ToString() == propertyValue)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task SendRunes<T>(params T[] runes) where T : Rune
        {
            foreach (var rune in runes)
            {
                //todo ensure message is taken
                if (RuneCount >= MaxRuneCount) TryRemoveRandomMessage();

                var bag = GetOrCreateBag(rune.Name);
                bag.Add(rune);
            }
            await Task.CompletedTask;
        }

        private void TryRemoveRandomMessage()
        {
            var bags = RuneBags.Where(r => r.Value.Count > 0)
                               .ToList();
            Rune result;
            bags[new Random().Next(0, bags.Count - 1)].Value.TryTake(out result);
        }

        private ConcurrentBag<Rune> GetOrCreateBag(string runeName)
        {
            if (!RuneBags.ContainsKey(runeName))
                RuneBags[runeName] = new ConcurrentBag<Rune>();
            return RuneBags[runeName];
        }
    }

    public static class Extensions
    {
        public static Rune PeekRandom(this ConcurrentBag<Rune> bag, Func<Rune, bool> predicate)
        {
            var list = bag.Where(predicate).ToList();
            return list[new Random().Next(0, list.Count - 1)];
        }
    }
}