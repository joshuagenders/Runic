using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.WorkGenerator
{
    public class WorkConsumer : IConsumer<Work>
    {
        private readonly BlockingCollection<Work> _workQueue;
        private readonly IPersonFactory _personFactory;

        public bool Closed => _workQueue.IsAddingCompleted;
        public int Count => _workQueue?.Count ?? 0;

        private IDatetimeService _datetimeService { get; set; }

        public WorkConsumer(
            IProducerConsumerCollection<Work> workCollection,
            IDatetimeService datetimeService,
            IPersonFactory personFactory)
        {
            _workQueue = new BlockingCollection<Work>(workCollection);
            _datetimeService = datetimeService;
            _personFactory = personFactory;
        }

        public void EnqueueTask(Work workItem)
        {
            _workQueue.Add(workItem);
        }

        private async Task ProcessPlan(WorkContext stateInfo)
        {
            if (stateInfo == null)
                return;
            var person = _personFactory.GetPerson(stateInfo.Work.Journey);
            await person.PerformJourneyAsync(stateInfo.Work.Journey, stateInfo.Ctx);
        }

        public async Task ProcessCallback(object stateInfo)
        {
            await ProcessPlan((WorkContext)stateInfo);
        }

        public void ProcessQueue(CancellationToken ctx)
        {
            foreach (var work in _workQueue.GetConsumingEnumerable())
            {
                var context = new WorkContext()
                {
                    Work = work,
                    Ctx = ctx
                };
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(
                        async delegate {
                            await ProcessCallback(context);
                        }));

                if (ctx.IsCancellationRequested || _workQueue.Count == 0)
                {
                    Close();
                    break;
                }
            }
        }

        public void Close()
        {
            _workQueue.CompleteAdding();
        }
    }
}
