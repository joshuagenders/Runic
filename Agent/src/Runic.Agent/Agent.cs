using Autofac;
using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Configuration;
using Runic.Agent.FlowManagement;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using Runic.Agent.Shell;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Agent
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task Start(string[] args)
        {
            AgentConfiguration.LoadConfiguration(args);
            var startup = new Startup();
            var container = startup.Register();

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

            await Execute(container, cts.Token);
        }

        private async Task Execute(IContainer container, CancellationToken ct)
        {
            // resolve ioc
            var pluginProvider = container.Resolve<IPluginProvider>();
            var messagingService = container.Resolve<IMessagingService>();
            //todo subscribe message handlers

            var flows = container.Resolve<Flows>();
            var pluginManager = container.Resolve<PluginManager>();
            pluginManager.RegisterProvider(pluginProvider);

            // start agent
            var agentService = new AgentService(pluginManager, flows);
            var serviceCts = new CancellationTokenSource();
            var agentTask = agentService.Run(messagingService, ct: serviceCts.Token);

            // start shell
            var shell = new AgentShell(agentService, flows, pluginManager);
            var cmdTask = shell.ProcessCommands(ct).ContinueWith(result =>
            {
                // cancel agent when shell exits
                serviceCts.Cancel();
            }, ct);

            // wait for shell and agent to complete
            await Task.WhenAll(cmdTask, agentTask).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    _logger.Error(t.Exception);
                    Console.WriteLine("An error occured. Agent Exiting.");
                }
                else
                {
                    Console.WriteLine("Agent Exiting.");
                }
                Thread.Sleep(5000);
            });
        }
    }
}
