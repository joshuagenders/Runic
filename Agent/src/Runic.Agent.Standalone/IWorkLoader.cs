using Runic.Agent.Core.WorkGenerator;
using System.Collections.Generic;

namespace Runic.Agent.Standalone
{
    public interface IWorkLoader
    {
        IEnumerable<Work> GetWork(Configuration configuration);
    }
}
