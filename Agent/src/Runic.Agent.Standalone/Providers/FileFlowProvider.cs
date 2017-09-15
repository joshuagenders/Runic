using Newtonsoft.Json;
using Runic.Agent.Framework.Models;
using System.IO;

namespace Runic.Agent.Standalone.Providers
{
    public class FileFlowProvider : IFlowProvider
    {
        public Flow GetFlow(string key)
        {
            if (!File.Exists(key))
            {
                throw new FileNotFoundException($"Flow not found at {key}");
            }
            return JsonConvert.DeserializeObject<Flow>(File.ReadAllText(key));
        }
    }
}
