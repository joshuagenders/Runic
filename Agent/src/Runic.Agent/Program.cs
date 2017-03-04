using Autofac;
using Newtonsoft.Json;
using Runic.Agent.Configuration;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Program
    {
        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            AgentConfiguration.LoadConfiguration(args);

            var cts = new CancellationTokenSource();
            try
            {
                var lifetimeMilliseconds = AgentConfiguration.LifetimeSeconds * 1000;
                if (lifetimeMilliseconds != 0)
                    cts.CancelAfter(lifetimeMilliseconds);
            }
            catch (OverflowException)
            {
                cts.CancelAfter(int.MaxValue);
            }

            var startup = new Startup();
            Task.Run(() => new Program().Execute(args, startup, cts.Token), cts.Token).ContinueWith(t =>
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
            }, cts.Token).Wait(cts.Token);

            ProcessCommands(cts.Token);
        }

        private static int ProcessCommands(CancellationToken cts)
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    var input = Console.ReadLine().Split();
                    if (input.Any())
                    {
                        switch (input[0])
                        {
                            case "setthread":

                                break;
                        }
                    }
                }
                catch
                {
                    return 1;
                }

            }

            return 0;
        }

        private async Task Execute(string[] args, IStartup startup, CancellationToken ct)
        {
            Container = startup.RegisterDependencies();
            startup.ConfigureApplication();

            var events = GetCompletionEvents();
            var tokenSources = new CancellationTokenSource[AgentConfiguration.MaxThreads];
            var executionService = Container.Resolve<IMessagingService>();

            while (!ct.IsCancellationRequested)
            {
                //wait for free thread slot
                var index = WaitHandle.WaitAny(events);
                var message = await executionService.ReceiveRequest(ct);

                if (message == null)
                    continue;

                tokenSources[index] = new CancellationTokenSource();
                tokenSources[index].CancelAfter(message.TimeoutMilliseconds);
                RunTest(message, events[index], tokenSources[index].Token);
            }

            CancelAll(tokenSources);
        }

        private async void RunTest(ExecutionRequest message, ManualResetEvent completionEvent,
            CancellationToken ct = default(CancellationToken))
        {
            var testOptions = new TestOptions
            {
                LockToThread = message.LockToThread,
                StepDelayMilliseconds = message.StepDelayMilliseconds
            };
            var virtualUser = new VirtualUser(testOptions, message.TestName);

            await Task.Run(() => { virtualUser.StartThread(ct, completionEvent); }, ct);
        }

        private ManualResetEvent[] GetCompletionEvents()
        {
            var events = new ManualResetEvent[AgentConfiguration.MaxThreads];
            for (var i = 0; i < AgentConfiguration.MaxThreads; i++)
                events[i] = new ManualResetEvent(true);
            return events;
        }

        private void CancelAll(CancellationTokenSource[] tokenSources)
        {
            foreach (CancellationTokenSource t in tokenSources)
                t?.Cancel();
        }
    }
}