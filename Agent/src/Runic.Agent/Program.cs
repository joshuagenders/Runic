using Autofac;
using Runic.Agent.Configuration;
using Runic.Agent.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Runic.Agent.Service;
using Runic.Agent.Shell;
using Runic.Agent.AssemblyManagement;

namespace Runic.Agent
{
    public class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AgentConfiguration.LoadConfiguration(args);
            var startup = new Startup();
            var container = startup.RegisterDependencies();

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
            
            Task.Run(() => new Program().Execute(args, container, cts.Token), cts.Token).ContinueWith(t =>
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

       
        private async Task Execute(string[] args, IContainer container, CancellationToken ct)
        {
            // resolve ioc
            var pluginProvider = container.Resolve<IPluginProvider>();
            var messagingService = container.Resolve<IMessagingService>();

            // start agent
            var agentService = new AgentService(pluginProvider);
            var serviceCts = new CancellationTokenSource();
            var agentTask = agentService.Run(messagingService, ct: serviceCts.Token);

            // start shell
            var shell = new AgentShell(agentService);
            var cmdTask = shell.ProcessCommands(ct).ContinueWith(result =>
            {
                // cancel agent when shell exits
                serviceCts.Cancel();
            }, ct);

            // wait for shell and agent to complete
            await Task.WhenAll(cmdTask, agentTask);
        }
    }
}