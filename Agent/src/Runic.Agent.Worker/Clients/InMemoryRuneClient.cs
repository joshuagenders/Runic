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
        public int MaxRuneCount { get; set; } = 1024;
        public int RuneCount => RuneBags.Sum(q => q.Value.Count);

        public InMemoryRuneClient()
        {
            RuneBags = new ConcurrentDictionary<string, ConcurrentBag<Rune>>();
        }

        public async Task<RuneQuery> GetRunes(RuneQuery query)
        {
            var bag = GetOrCreateBag(query.RuneName);
            query.Result = bag.PeekRandom(r => RuneHasMatchingProperties(r, query.RequiredProperties));
            return await Task.FromResult(query);
        }

        public async Task<RuneQuery> TakeRunes(RuneQuery query)
        {
            var bag = GetOrCreateBag(query.RuneName);
            query.Result = bag.TakeRandom(r => RuneHasMatchingProperties(r, query.RequiredProperties));
            return await Task.FromResult(query);
        }

        private bool RuneHasMatchingProperties(Rune rune, Dictionary<string, string> properties)
        {
            if (properties == null)
                return true;

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

        private bool PropertyExists(object obj, string propertyName)
        {
            var split = propertyName.Split('.');
            if (split.Length > 1)
            {
                var prop = obj.GetType().GetProperties().Where(o => o.Name == split[0]);
                if (prop.Any())
                {
                    return PropertyExists(prop.GetType().GetProperty(split[0]).GetValue(obj), propertyName.Substring(0, propertyName.IndexOf('.')));
                }
                else
                {
                    return false;
                }
            }
            //base case
            return obj.GetType().GetProperties().Select(p => p.Name).Contains(propertyName);
        }

        private bool PropertyExists(object obj, string propertyName, string propertyValue)
        {
            var split = propertyName.Split('.');
            if (split.Length > 1)
            {
                var prop = obj.GetType().GetProperties().Where(o => o.Name == split[0]);
                if (prop.Any())
                {
                    return PropertyExists(prop.GetType().GetProperty(split[0]).GetValue(obj), propertyName.Substring(0, propertyName.IndexOf('.')), propertyValue);
                }
                else
                {
                    return false;
                }
            }
            //base case
            var baseProp = obj.GetType().GetProperties().Where(o => o.Name == propertyName);
            if (baseProp.Any() && baseProp.FirstOrDefault().GetValue(obj).ToString() == propertyValue)
            {
                return true;
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

        public static Rune TakeRandom(this ConcurrentBag<Rune> bag, Func<Rune, bool> predicate)
        {
            var list = bag.Where(predicate).ToList();
            Rune rune = null;
            List<Rune> nonMatchingRunes = new List<Rune>();
            while (rune == null)
            {
                var res = bag.Take(1).Single();
                if (res == null || bag.Count <= 0)
                    break;

                if (predicate(res))
                {
                    rune = res;
                }
                else
                {
                    nonMatchingRunes.Add(res);
                }
            }
            nonMatchingRunes.ForEach(r => bag.Add(r));
            return rune;
        }
    }
}