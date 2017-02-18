using Runic.Clients;
using Runic.Core.Messaging;
using System.Threading.Tasks;

namespace Runic.Data
{
    public class Runes
    {
        public static IRuneClient Client { get; set; }

        public static async Task<RuneQuery> Retrieve(RuneQuery query) 
        {
            return await Client.RetrieveRunes(query);
        }

        public static void Mine<T>(params T[] runes) where T : Rune
        {
            Client.SendRunes(runes);
        }
    }
}