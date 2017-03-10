using Autofac;
using Newtonsoft.Json;
using Runic.Agent.Configuration;
using Runic.Agent.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Runic.Agent.Service;
using Runic.Agent.Shell;

namespace Runic.Agent
{
    public class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AgentConfiguration.LoadConfiguration(args);
            IoC.RegisterDependencies(new Startup());

            var cts = new CancellationTokenSource();
            try
            {
                var lifetimeMilliseconds = AgentConfiguration.Instance.LifetimeSeconds * 1000;
                if (lifetimeMilliseconds != 0)
                    cts.CancelAfter(lifetimeMilliseconds);
            }
            catch (OverflowException)
            {
                _logger.Warn("Overflow on agent lifetime timeout value. using max int value.");
                cts.CancelAfter(int.MaxValue);
            }
            
            Task.Run(() => new Program().Execute(args, cts.Token), cts.Token).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    _logger.Error(t.Exception);
                    Console.WriteLine("An error occured. Exiting.");
                }
                else
                {
                    Console.WriteLine("Load testing completed. Exiting.");
                }
                Thread.Sleep(15000);
            }, cts.Token).Wait(cts.Token);
        }

       
        private async Task Execute(string[] args, CancellationToken ct)
        {   
            var agentService = IoC.Container.Resolve<IAgentService>();
            var shell = new AgentShell(agentService);

            var serviceCts = new CancellationTokenSource();
            var agentTask = agentService.Run(ct: serviceCts.Token);

            var cmdTask = shell.ProcessCommands(ct).ContinueWith(result =>
            {
                serviceCts.Cancel();
            }, ct);

            await Task.WhenAll(cmdTask, agentTask);
        }
    }
}