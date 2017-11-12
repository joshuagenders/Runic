using Runic.Agent.Core.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class Runner
    {
        private readonly TestPlanProducer _testPlanProducer;
        private readonly TestPlanConsumer _testPlanConsumer;
        private readonly TaskFactory _taskFactory;

        public Runner(TestPlanProducer testPlanProducer, TestPlanConsumer testPlanConsumer, ICoreConfiguration config)
        {
            _taskFactory = new TaskFactory();
            _testPlanProducer = testPlanProducer;
            _testPlanConsumer = testPlanConsumer;
        }

        public async Task Start(CancellationToken ctx)
        {
            await Task.WhenAll(_testPlanProducer.ProduceWorkItems(ctx), _taskFactory.StartNew(() => _testPlanConsumer.Start(ctx)));
        }
    }
}
