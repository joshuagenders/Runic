using System.Threading.Tasks;
using Runic.Core.Models;

namespace Runic.Clients
{
    public interface IRuneClient
    {
        Task<RuneQuery> RetrieveRunes(RuneQuery query);
        void SendRunes<T>(params T[] runes);
    }
}