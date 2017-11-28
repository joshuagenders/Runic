using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.Standalone;

namespace Runic.Agent.FunctionalTest.TestUtility
{
    public class TestEnvironment
    {
        public IApplication Application { get; set; }
        public IWorkLoader WorkLoader { get; set; }
        public IProducer<Work> WorkProducer { get; set; }
        public IConsumer<Work> WorkConsumer{ get; set; }
        public IAssemblyManager AssemblyManager { get; set; }
        public IRunner<Work> Runner { get; set; }
    }
}
