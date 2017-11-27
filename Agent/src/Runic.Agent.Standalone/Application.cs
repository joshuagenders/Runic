using Runic.Agent.Core.WorkGenerator;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone
{
    public class Application : IApplication
    {
        private readonly Configuration _config;
        private readonly IProducer<Work> _workProducer;
        private readonly IWorkLoader _workLoader;
        private readonly IRunner<Work> _runner;

        public Application(
            Configuration config, 
            IProducer<Work> workProducer, 
            IWorkLoader workLoader, 
            IRunner<Work> runner
        )
        {
            _config = config;
            _workProducer = workProducer;
            _workLoader = workLoader;
            _runner = runner;
        }

        public async Task Run(CancellationToken ctx)
        {
            _workLoader.GetWork(_config)
                       .ToList()
                       .ForEach(i => _workProducer.AddUpdateWorkItem(i.Journey.Name, i));
            await _runner.Start(ctx);
        }
    }
}
