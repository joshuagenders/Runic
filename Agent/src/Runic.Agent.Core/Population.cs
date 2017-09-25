using Runic.Agent.Core.Configuration;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class Population
    {
        private readonly IPersonAttributeService _personAttributeService;
        private readonly SemaphoreSlim _semaphore;
        private readonly ICoreConfiguration _config;
        private readonly BlockingCollection<TestPlan> _workQueue;
        private readonly IPersonFactory _personFactory;

        public readonly int MaxActivePopulation;
        private readonly IDatetimeService _datetimeService;

        public Population(
            IPersonAttributeService personAttributeService,
            IProducerConsumerCollection<TestPlan> testPlanTaskCollection,
            IPersonFactory personFactory,
            IDatetimeService datetimeService,
            ICoreConfiguration config)
        {
            _personAttributeService = personAttributeService;
            _personFactory = personFactory;
            _config = config;
            _datetimeService = datetimeService;
            MaxActivePopulation = config.MaxActivePopulation;
            _semaphore = new SemaphoreSlim(MaxActivePopulation);
            _workQueue = new BlockingCollection<TestPlan>(testPlanTaskCollection);
        }

        private async Task<bool> RequestPerson(CancellationToken ctx = default(CancellationToken))
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(_config.PopulationRequestTimeoutSeconds * 1000);
            return await Task.Run(async () =>
            {
                try
                {
                    await _semaphore.WaitAsync(cts.Token);
                    return true;
                }
                catch (TaskCanceledException)
                {
                    //stats, log or raise event, observer etc
                    return false;
                }
            }, ctx);
        }

        private void ReleasePerson()
        {
            _semaphore.Release();
        }

        //entry point
        public void AddTask(TestPlan testPlan)
        {
            _workQueue.Add(testPlan);
            RequestPopulationGrowth();
        }

        public void RequestPopulationGrowth()
        {

        }

        private List<Task> tasks { get; set; }
        public async Task Run(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested)
            {
                if (tasks.Count < MaxActivePopulation && _workQueue.Count > 0)
                {
                    var taskFactory = new TaskFactory();
                    var exceptions = new ConcurrentQueue<Exception>();
                    tasks.Add(taskFactory.StartNew(async () =>
                    {
                        TestPlan testPlan = _workQueue.Take();
                        if (testPlan != null && await RequestPerson())
                        {
                        //reconsider this exception handling
                        //errors will have to wait until all threads complete until being bubbled up
                        //but wrapping the parallel for will kill all successful and failing
                        try { await ExecuteTestPlan(testPlan, ctx); }
                            catch (Exception e) { exceptions.Enqueue(e); }
                            finally { ReleasePerson(); }
                        }
                    }));
                }
                else
                {
                    ctx.ThrowIfCancellationRequested();
                    await _datetimeService.WaitMilliseconds(1000, ctx);
                }
            }
        }

        private async Task ExecuteTestPlan(TestPlan testPlan, CancellationToken ctx)
        {
            var person = _personFactory.GetPerson(testPlan.Journey);
            await person.PerformJourneyAsync(testPlan.Journey, ctx);
        }
    }
}
