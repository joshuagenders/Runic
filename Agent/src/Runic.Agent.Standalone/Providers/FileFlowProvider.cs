using Newtonsoft.Json;
using Runic.Agent.Framework.Models;
using System.IO;

namespace Runic.Agent.Standalone.Providers
{
    public class FileFlowProvider : IFlowProvider
    {
        public Journey GetFlow(string key)
        {
            if (!File.Exists(key))
            {
                throw new FileNotFoundException($"Flow not found at {key}");
            }
            return JsonConvert.DeserializeObject<Journey>(File.ReadAllText(key));
        }
    }
}
