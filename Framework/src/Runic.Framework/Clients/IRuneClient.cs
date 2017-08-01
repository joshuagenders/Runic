using System.Threading.Tasks;
using Runic.Framework.Models;

namespace Runic.Framework.Clients
{
    public interface IRuneClient
    {
        Task<RuneQuery> GetRunes(RuneQuery query);
        Task<RuneQuery> TakeRunes(RuneQuery query);
        Task SendRunes<T>(params T[] runes) where T : Rune;
    }
}