using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Service;
using Runic.Core.Models;
using static Runic.Agent.Shell.AgentShell.ReturnCodes;

namespace Runic.Agent.Shell
{
    public class AgentShell
    {
        public enum ReturnCodes
        {
            SUCCESS = 0,
            ERROR = 1
        }

        private Dictionary<string, Func<string[], Task<int>>> _handlers { get; set; }
        private bool _return { get; set; }
        private IAgentService _agentService { get; }
        private CancellationToken _cancellationToken { get; set; }

        public AgentShell(IAgentService agentService)
        {
            _return = false;
            _agentService = agentService;
            RegisterHandlers();
        }

        /// <summary>
        /// setthread plan=OrderFlow threadcount=1 [pluginprovider=Runic.Agent.AssemblyManagement.FilePluginProvider] [pluginkey=Runic.ExampleTest] 
        /// runfunc name=Login [pluginprovider=Runic.Agent.AssemblyManagement.FilePluginProvider] [pluginkey=Runic.ExampleTest]
        /// load pluginprovider=Runic.Agent.AssemblyManagement.FilePluginProvider pluginkey=Runic.ExampleTest
        /// unload Runic.ExampleTest
        /// lf (list functions)
        /// lp (list plugins)
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task ProcessCommands(CancellationToken ct)
        {
            _cancellationToken = ct;
            while (!ct.IsCancellationRequested && !_return)
            {
                try
                {
                    string[] input = Console.ReadLine().Split();
                    if (!input.Any()) continue;
                    if (_handlers.ContainsKey(input[0]))
                    {
                        await _handlers[input[0]](input);
                    }
                    else
                    {
                        Console.WriteLine($"No handler was found for the given command '{input[0]}'");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encountered error");
                }
            }
        }

        private void RegisterHandlers()
        {
            _handlers = new Dictionary<string, Func<string[], Task<int>>>()
            {
                {
                    "setthread", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        
                        if (vals.ContainsKey("pluginkey"))
                        {
                            await LoadPlugin(vals["pluginkey"], _cancellationToken);
                        }

                        return await SetThreadLevel(vals["plan"], int.Parse(vals["threadCount"]),_cancellationToken);
                    }
                },
                {
                    "load", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await LoadPlugin(vals["pluginkey"], _cancellationToken);
                        return (int) SUCCESS;
                    }
                },
                {
                    "help", async (input) =>
                    {
                        await Task.Run(() => Console.WriteLine("load, help, exit, setthread"), _cancellationToken);
                        return (int)SUCCESS;
                    }
                },
                {
                    "exit", async (input) => 
                    {
                        await Task.Run(() => _return = true);
                        return (int) SUCCESS;
                    }
                }
            };
        }

        private async Task LoadPlugin(string pluginKey, CancellationToken ct)
        {
            await Task.Run(() => PluginManager.LoadPlugin(pluginKey), ct);
        }

        private async Task<int> SetThreadLevel(string flowName, int threadCount, CancellationToken ct)
        {
            await _agentService.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = flowName,
                ThreadLevel = threadCount
            }, ct);
            return (int)SUCCESS;
        }
    }
}
