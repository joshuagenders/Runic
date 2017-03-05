using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Shell
{
    public static class Extensions
    {
        public static Dictionary<string, string> FromKeywordToDictionary(this string[] val)
        {
            return val.Select(s => s.Split('=')).ToDictionary(split => split[0], split => split[1]);
        }
    }
}
