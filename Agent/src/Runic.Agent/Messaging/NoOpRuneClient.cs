using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Services
{
    public class NoOpRuneClient : IRuneClient
    {
        public Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            return Task.FromResult(query);
        }

        public Task SendRunes<T>(params T[] runes) where T : Rune
        {
            return Task.CompletedTask;
        }
    }
}
