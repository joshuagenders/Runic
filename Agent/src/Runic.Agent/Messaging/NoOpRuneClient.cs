﻿using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Messaging
{
    public class NoOpRuneClient : IRuneClient
    {
        public Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            return null;
        }

        public void SendRunes<T>(params T[] runes) where T : Rune
        {
        }
    }
}
