using System;
using System.Threading.Tasks;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace Runic.Agent.Standalone.Clients
{
    public class InMemoryRuneClient : IRuneClient
    {
        public ConcurrentDictionary<Type, ConcurrentBag<Rune>> RuneBags { get; set; }
        public int MaxRuneCount { get; set; }
        public int RuneCount => RuneBags.Sum(q => q.Value.Count);

        public InMemoryRuneClient(int maxRuneCount = 1024)
        {
            RuneBags = new ConcurrentDictionary<Type, ConcurrentBag<Rune>>();
            MaxRuneCount = maxRuneCount;
        }
        
        public async Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            //todo
            return await Task.FromResult(query);
        }
        
        public async Task SendRunes<T>(params T[] runes) where T : Rune
        {
            foreach (var rune in runes)
            {
                //todo ensure message is taken
                if (RuneCount >= MaxRuneCount) TryRemoveRandomMessage();

                var bag = GetOrCreateBag(rune.GetType());
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

        private ConcurrentBag<Rune> GetOrCreateBag(Type type)
        {
            if (!RuneBags.ContainsKey(type))
                RuneBags[type] = new ConcurrentBag<Rune>();
            return RuneBags[type];
        }
    }
}
