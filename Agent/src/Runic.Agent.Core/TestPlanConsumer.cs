using System.Collections.Concurrent;
using System.Threading;

namespace Runic.Agent.Core
{
    public class TestPlanConsumer
    {
        private readonly IPersonFactory _personFactory;
        private readonly BlockingCollection<TestPlan> _workQueue;
        public bool Closed { get; private set; }
        public int Count => _workQueue?.Count ?? 0;

        public TestPlanConsumer(
            IProducerConsumerCollection<TestPlan> testPlanTaskCollection,
            IPersonFactory personFactory)
        {
            _personFactory = personFactory;
            _workQueue = new BlockingCollection<TestPlan>(testPlanTaskCollection);
        }

        public void EnqueueTask(TestPlan testPlan, CancellationToken ctx)
        {
            _workQueue.Add(testPlan);
        }

        private void ProcessPlan(object stateInfo)
        {
            var si = (TestPlanInfo)stateInfo;
            var person = _personFactory.GetPerson(si.TestPlan.Journey);
            person.PerformJourneyAsync(si.TestPlan.Journey, si.Ctx).Wait();
        }

        public void Start(CancellationToken ctx)
        {
            foreach (var testPlan in _workQueue.GetConsumingEnumerable())
            { 
                ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessPlan), new TestPlanInfo(){ TestPlan = testPlan, Ctx = ctx });
                if (ctx.IsCancellationRequested)
                    break;
            }
        }

        public void Close()
        {
            Closed = true;
            _workQueue.CompleteAdding();
        }

        class TestPlanInfo
        {
            public CancellationToken Ctx { get; set; }
            public TestPlan TestPlan { get; set; }
        }
    }
}
