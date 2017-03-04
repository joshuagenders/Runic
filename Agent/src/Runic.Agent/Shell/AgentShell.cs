using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Service;
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
        private IAgentService _agentService { get; set; }
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
        public async Task<int> ProcessCommands(CancellationToken ct)
        {
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

            return (int)SUCCESS;
        }

        public void RegisterHandlers()
        {
            _handlers = new Dictionary<string, Func<string[], Task<int>>>()
            {
                {
                    "setthread", async (input) =>
                    {
                        var vals = input.ToKeywordDictionary();
                        
                        if (vals.ContainsKey("pluginkey"))
                        {
                            await LoadPlugin(vals["pluginkey"], vals["pluginprovider"]);
                        }

                        return await SetThreadLevel(vals["plan"], int.Parse(vals["threadCount"]));
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

        private async Task LoadPlugin(string pluginKey, string pluginProvider)
        {
        }

        private async Task<int> SetThreadLevel(string plan, int threadCount)
        {
            //todo
            await Task.Run(() => { });
            return (int)SUCCESS;
        }
    }
}
