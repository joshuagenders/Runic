using Runic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Clients
{
    public interface IRuneClient
    {
        Task<List<Rune>> RetrieveRunes(params RuneQuery[] queries);
        void SendRunes(params Rune[] runes);
    }
}
