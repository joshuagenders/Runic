using Newtonsoft.Json;
using Runic.Agent.Core.WorkGenerator;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Runic.Agent.Standalone
{
    public class WorkLoader : IWorkLoader
    {
        public IEnumerable<Work> GetWork (Configuration configuration)
        {
            var files = Directory.GetFiles(configuration.WorkFolderPath)
                                 .Where(f => f.EndsWith(".work.json"));
            if (!files.Any())
            {
                throw new NoWorkException(
                    $"No work files (.work.json) were located in the configured directory: {configuration.WorkFolderPath}");
            }
            
            var work = files.Select(File.ReadAllText)
                            .Select(JsonConvert.DeserializeObject<Work>)
                            .ToList();
            return work;
        }
    }
}
