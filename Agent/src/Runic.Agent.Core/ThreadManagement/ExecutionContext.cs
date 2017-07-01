using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Metrics;

namespace Runic.Agent.Core.ThreadManagement
{
    public class ExecutionContext
    {
        public IPluginManager pluginManager { get; set; }
        public IFlowManager flowManager { get; set; }
        public IStats stats { get; set; }
        public IDataService dataService { get; set; }
        public ExecutionContext()
        {

        }
    }
}
