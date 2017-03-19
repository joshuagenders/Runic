using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Runic.Framework.Models;
using System.Linq;

namespace Runic.Framework.Clients
{
    public class InMemoryClient : IRuneClient
    {
        private InMemoryStore _store { get; set; }

        public InMemoryClient()
        {
            _store = new InMemoryStore();
        }

        public async Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            return await Task.Run(() => _store.Retrieve(query));
        }

        public async Task SendRunes<T>(params T[] runes) where T : Rune
        {
            List<Rune> runesList = new List<Rune>(runes);
            await Task.Run(() => _store.Store(runesList));
        }
    }
}
