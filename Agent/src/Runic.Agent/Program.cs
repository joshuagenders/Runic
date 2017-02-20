using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Core.Models;

namespace Runic.Agent
{
    public class Program
    {
        public Program()
        {
            MaxThreads = int.Parse(AgentConfiguration.Config["Agent:MaxThreads"]);
        }

        private int MaxThreads { get; }

        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            AgentConfiguration.LoadConfiguration(args);

            var startup = new Startup();

            var lifetimeMilliseconds = int.Parse(AgentConfiguration.Config["Agent:LifetimeSeconds"]) * 1000;
            if (lifetimeMilliseconds != 0)
                cts.CancelAfter(lifetimeMilliseconds);

            new Program().Execute(args, startup, cts.Token).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(t.Exception));

                    Console.WriteLine("An error occured. Exiting.");
                }
                else
                {
                    Console.WriteLine("Load testing completed. Exiting.");
                }
                Thread.Sleep(5000);
            }).Wait();
        }

        public async Task Execute(string[] args, IStartup startup, CancellationToken ct)
        {
            Container = startup.RegisterDependencies();
            startup.ConfigureApplication();

            var events = GetCompletionEvents();
            var tokenSources = new CancellationTokenSource[MaxThreads];
            var executionService = Container.Resolve<IMessagingService>();

            while (!ct.IsCancellationRequested)
            {
                //wait for free thread slot
                var index = WaitHandle.WaitAny(events);
                var message = await executionService.ReceiveRequest();

                if (message == null)
                    continue;

                tokenSources[index] = new CancellationTokenSource();
                tokenSources[index].CancelAfter(message.TimeoutMilliseconds);
                RunTest(message, events[index], tokenSources[index].Token);
            }

            CancelAll(tokenSources);
        }

        public async void RunTest(ExecutionRequest message, ManualResetEvent completionEvent,
            CancellationToken ct = default(CancellationToken))
        {
            var testOptions = new TestOptions
            {
                LockToThread = message.LockToThread,
                StepDelayMilliseconds = message.StepDelayMilliseconds
            };
            var virtualUser = new VirtualUser(testOptions, message.TestName);

            await Task.Run(() => { virtualUser.StartThread(ct, completionEvent); });
        }

        public ManualResetEvent[] GetCompletionEvents()
        {
            var events = new ManualResetEvent[MaxThreads];
            for (var i = 0; i < MaxThreads; i++)
                events[i] = new ManualResetEvent(true);
            return events;
        }

        public void CancelAll(CancellationTokenSource[] tokenSources)
        {
            for (var i = 0; i < tokenSources.Length; i++)
                tokenSources[i]?.Cancel();
        }
    }
}