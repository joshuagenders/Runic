using Autofac;
using Newtonsoft.Json;
using Runic.Agent.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Runic.Core.Messaging;
using Runic.Agent.Harness;

namespace Runic.Agent
{
    public class Program
    {
        private int _maxThreads { get; set; }

        private static IContainer _container { get; set; }

        public Program()
        {
            _maxThreads = int.Parse(AgentConfiguration.Config["Agent:MaxThreads"]);
        }

        public static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            AgentConfiguration.LoadConfiguration(args);

            var startup = new Startup();

            var lifetimeMilliseconds = int.Parse(AgentConfiguration.Config["Agent:LifetimeSeconds"]) * 1000;
            if (lifetimeMilliseconds != 0)
            {
                cts.CancelAfter(lifetimeMilliseconds);
            }

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
            _container = startup.RegisterDependencies();
            startup.ConfigureApplication();

            var events = GetCompletionEvents();
            CancellationTokenSource[] tokenSources = new CancellationTokenSource[_maxThreads];
            var executionService = _container.Resolve<IMessagingService>();

            while (!ct.IsCancellationRequested)
            {
                //wait for free thread slot
                int index = WaitHandle.WaitAny(events);
                var message = await executionService.ReceiveRequest();

                if (message == null)
                {
                    continue;
                }

                tokenSources[index] = new CancellationTokenSource();
                tokenSources[index].CancelAfter(message.TimeoutMilliseconds);
                RunTest(message, events[index], tokenSources[index].Token);
            }

            CancelAll(tokenSources);
        }

        public async void RunTest(ExecutionRequest message, ManualResetEvent completionEvent, CancellationToken ct = default(CancellationToken))
        {
            var testOptions = new TestOptions()
            {
                LockToThread = message.LockToThread,
                StepDelayMilliseconds = message.StepDelayMilliseconds
            };
            var virtualUser = new VirtualUser(testOptions, message.TestName);

            await Task.Run(() => { virtualUser.StartThread(ct, completionEvent); });
        }

        public ManualResetEvent[] GetCompletionEvents()
        {
            ManualResetEvent[] events = new ManualResetEvent[_maxThreads];
            for (int i = 0; i < _maxThreads; i++)
            {
                events[i] = new ManualResetEvent(true);
            }
            return events;
        }

        public void CancelAll(CancellationTokenSource[] tokenSources)
        {
            for (int i = 0; i < tokenSources.Length; i++)
            {
                tokenSources[i]?.Cancel();
            }
        }
    }
}
