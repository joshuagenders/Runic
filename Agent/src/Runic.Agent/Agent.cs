using Autofac;
using NLog;
using Runic.Agent.Configuration;
using Runic.Agent.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Agent
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task Start(string[] args, IStartup startup)
        {
            AgentConfiguration.LoadConfiguration(args);
            var container = startup.BuildContainer();

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

            //start service
            using (var scope = container.BeginLifetimeScope())
            {
                var agent = scope.Resolve<IAgentService>();
                await agent.Run(cts.Token);
            }
        }
    }
}
