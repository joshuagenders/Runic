using Runic.Framework.Clients;
using System;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Aws.Services
{
    public class DynamoRuneClient : IRuneClient
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
