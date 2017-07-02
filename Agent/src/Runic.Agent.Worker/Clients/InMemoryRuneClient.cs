using System;
using System.Threading.Tasks;
using Runic.Framework.Clients;
using Runic.Framework.Models;

namespace Runic.Agent.Worker.Clients
{
    //todo
    public class InMemoryRuneClient : IRuneClient
    {
        public Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            throw new NotImplementedException();
        }

        public Task SendRunes<T>(params T[] runes) where T : Rune
        {
            throw new NotImplementedException();
        }
    }
}
