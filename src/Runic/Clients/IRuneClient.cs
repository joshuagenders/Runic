using Runic.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runic.Clients
{
    interface IRuneClient
    {
        Task<HttpResponseMessage> SendRune(Rune rune);
    }
}
