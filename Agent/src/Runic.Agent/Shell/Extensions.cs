using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Shell
{
    public static class Extensions
    {
        public static Dictionary<string, string> FromKeywordToDictionary(this string[] val)
        {
            var result = new Dictionary<string, string>();
            val.ToList()
               .ForEach(i =>
            {
                if (i.Contains("="))
                {
                    var split = i.Split('=');
                    result[split[0]] = split[1];
                }
                else
                {
                    result[i] = i;
                }
            });
            return result;
        }
    }
}
