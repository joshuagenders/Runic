using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Service;
using Runic.Framework.Models;
using System.IO;

namespace Runic.Agent.Shell
{
    public class AgentShell
    {
        private Dictionary<string, Func<string[], Task>> _handlers { get; set; }
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
                    if (input[0] == "exit") return;

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
            _handlers = new Dictionary<string, Func<string[], Task>>()
            {
                {
                    "setthread", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        
                        if (vals.ContainsKey("pluginkey"))
                        {
                            await LoadPlugin(vals["pluginkey"], _cancellationToken);
                        }

                        await SetThreadLevel(vals["flow"], int.Parse(vals["threadcount"]),_cancellationToken);
                    }
                },
                {
                    "load", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await LoadPlugin(vals["pluginkey"], _cancellationToken);
                    }
                },
                {
                    "help", async (input) =>
                    {
                        await Task.Run(() =>
                        {
                            Console.WriteLine("setthread pluginkey=[string] flow=[filename] threadcount=[int]");
                            Console.WriteLine("sets the thread level for a flow");
                            Console.WriteLine("Starts or stops a flow when threadCount moves to and from 0");
                            Console.WriteLine("load pluginkey=");
                            Console.WriteLine("Loads an assembly using the IPluginProvider configured in the assembly");
                            Console.WriteLine("flows - list the available flows");
                            Console.WriteLine("functions - list the available functions");
                            Console.WriteLine("help - display available command formats");
                            Console.WriteLine("exit - Stop all threads and exit the agent");
                            
                        }, _cancellationToken);
                    }
                }
            };
        }

        private async Task LoadPlugin(string pluginKey, CancellationToken ct)
        {
            await Task.Run(() => 
                PluginManager.LoadPlugin(
                    pluginKey, 
                    new FilePluginProvider(Directory.GetCurrentDirectory())), ct);
        }

        private async Task SetThreadLevel(string flowName, int threadCount, CancellationToken ct)
        {
            await _agentService.SetThreadLevel(new SetThreadLevelRequest()
            {
                FlowName = flowName,
                ThreadLevel = threadCount
            }, ct);
        }
    }
}
