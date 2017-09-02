using System;
using System.Threading.Tasks;
using Runic.Framework.Clients;
using Runic.Framework.Models;

namespace Runic.Agent.Aws.Services
{
    public class SnsRuneClient : IRuneClient
    {
        public Task<RuneQuery> GetRunes(RuneQuery query)
        {
            throw new NotImplementedException();
        }

        public Task SendRunes<T>(params T[] runes) where T : Rune
        {
            throw new NotImplementedException();
        }

        public Task<RuneQuery> TakeRunes(RuneQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
