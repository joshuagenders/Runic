using Runic.Core.Models;
using System.Threading.Tasks;

namespace Runic.Clients
{
    public interface IRuneClient
    {
        Task<RuneQuery> RetrieveRunes(RuneQuery query);
        void SendRunes<T>(params T[] runes);
    }
}
