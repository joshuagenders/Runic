using Newtonsoft.Json;
using Runic.Clients;
using Runic.Query;
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
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Rune>(content);
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
            var response = await Client.RetrieveRunes(runeQueries[0]);
            var content = await response.Content.ReadAsStringAsync();
            return new List<Rune>() { JsonConvert.DeserializeObject<Rune>(content) };
        }

        public static void Mine(params Rune[] runes)
        {
            Client.SendRunes(runes);
        }
    }
}
