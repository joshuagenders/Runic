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

        public async void SendRunes<T>(params T[] runes) where T : Rune
        {
            await Task.Run(() => _store.Store(runes.ToList() as List<Rune>));
        }
    }
}
