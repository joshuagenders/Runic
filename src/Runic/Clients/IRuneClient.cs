using Runic.Data;
using RunicCore.Query;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runic.Clients
{
    public interface IRuneClient
    {
        Task<List<Rune>> RetrieveRunes(params RuneQuery[] queries);
        Task<HttpResponseMessage> SendRunes(params Rune[] runes);
    }
}
