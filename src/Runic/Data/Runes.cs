using Runic.Clients;
using RunicCore.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runic.Data
{
    public class Runes
    {
        public static IRuneClient Client { get; set; }

        public static async Task<Rune> Retrieve(string runeName)
        {
            return await Retrieve(new RuneQuery()
            {
                RuneName = runeName,
                EnableRegex = false
            });
        }

        public static async Task<Rune> Retrieve(RuneQuery runeQuery)
        {
            var response = await Client.RetrieveRunes(runeQuery);
            return response[0];
        }

        public static async Task<List<Rune>> RetrieveMultiple(params string[] runeNames)
        {
            //TODO
            return await RetrieveMultiple(new RuneQuery()
            {
                RuneName = runeNames[0],
                EnableRegex = false
            });
        }

        public static async Task<List<Rune>> RetrieveMultiple(params RuneQuery[] runeQueries)
        {
            //TODO
            return await Client.RetrieveRunes(runeQueries[0]);
        }

        public static void Mine(params Rune[] runes)
        {
            Client.SendRunes(runes);
        }
    }
}
