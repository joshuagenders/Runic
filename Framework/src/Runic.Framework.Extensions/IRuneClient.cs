﻿using System.Threading.Tasks;
using Runic.Framework.Models;

namespace Runic.Framework.Extensions
{
    public interface IRuneClient
    {
        Task<RuneQuery> RetrieveRunes(RuneQuery query);
        Task SendRunes<T>(params T[] runes) where T : Rune;
    }
}