using Runic.Data;
using Runic.Query;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runic.Clients
{
    public interface IRuneClient
    {
        Task<HttpResponseMessage> RetrieveRunes(params RuneQuery[] queries);
        Task<HttpResponseMessage> SendRunes(params Rune[] runes);
    }
}
