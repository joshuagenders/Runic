using System.Threading.Tasks;
using Runic.Agent.Framework.Models;

namespace Runic.Agent.Framework.Clients
{
    public interface IRuneClient
    {
        Task<RuneQuery> GetRunes(RuneQuery query);
        Task<RuneQuery> TakeRunes(RuneQuery query);
        Task SendRunes<T>(params T[] runes) where T : Rune;
    }
}