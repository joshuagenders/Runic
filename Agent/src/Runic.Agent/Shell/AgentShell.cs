using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Service;
using Runic.Framework.Models;
using System.IO;
using Newtonsoft.Json;
using Runic.Agent.FlowManagement;

namespace Runic.Agent.Shell
{
    public class AgentShell
    {
        private Dictionary<string, Func<string[], Task>> _handlers { get; set; }
        private bool _return { get; set; }
        private IAgentService _agentService { get; }
        private CancellationToken _cancellationToken { get; set; }
        private readonly PluginManager _pluginManager;
        private readonly Flows _flows;

        public AgentShell(IAgentService agentService, Flows flows, PluginManager pluginManager)
        {
            _return = false;
            _agentService = agentService;
            _pluginManager = pluginManager;
            _flows = flows;
            RegisterHandlers();
        }

        public async Task ProcessCommands(CancellationToken ct)
        {
            WriteWelcome();
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
                    Console.WriteLine(JsonConvert.SerializeObject(e));
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
                        if (!vals.ContainsKey("flow")){
                            Console.WriteLine("missing parameter 'flow'");
                            return;
                        }
                        if (!vals.ContainsKey("threadcount")){
                            Console.WriteLine("missing parameter 'threadcount'");
                            return;
                        }

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
                        if (!vals.ContainsKey("pluginkey")){
                            Console.WriteLine("missing parameter 'pluginkey'");
                            return;
                        }
                        await LoadPlugin(vals["pluginkey"], _cancellationToken);
                    }
                },
                {
                    "functions", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await Task.Run(() =>
                        {
                            var functions = _pluginManager.GetAvailableFunctions();
                            functions.ForEach(f => Console.WriteLine(JsonConvert.SerializeObject(f)));
                        });
                    }
                },

                {
                    "import", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        if (!vals.ContainsKey("filepath")){
                            Console.WriteLine("missing parameter 'filepath'");
                            return;
                        }
                        await Task.Run(() =>
                        {
                            var text = File.ReadAllText(vals["filepath"]);
                            var flow = JsonConvert.DeserializeObject<Flow>(text);
                            _flows.AddUpdateFlow(flow);
                        });
                    }
                },
                {
                    "plugins", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await Task.Run(() =>
                        {
                            if (vals.ContainsKey("-v")){
                                Console.Write("Assemblies:");
                                _pluginManager.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));
                            }
                            Console.Write("Keys:");
                            _pluginManager.GetAssemblyKeys().ForEach(a => Console.WriteLine(a));
                        });
                    }
                },
                {
                    "flows", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await Task.Run(() =>
                        {
                            _flows.GetFlows()
                                  .ForEach(f =>
                                  {
                                    if (vals.ContainsKey("-v"))
                                          Console.WriteLine(JsonConvert.SerializeObject(f));
                                    else
                                          Console.WriteLine(f.Name);
                                    }
                                   );
                        });
                    }
                },
                {
                    "help", async (input) =>
                    {
                        await Task.Run(() =>
                        {
                            Console.WriteLine("=============================");
                            Console.WriteLine("            HELP");
                            Console.WriteLine("=============================");
                            Console.WriteLine("setthread pluginkey=[string] flow=[filename] threadcount=[int]");
                            Console.WriteLine("  Sets the thread level for a flow\n");
                            Console.WriteLine("  Starts or stops a flow when threadCount moves to and from 0\n");

                            Console.WriteLine("load pluginkey=[string]");
                            Console.WriteLine("  Loads an assembly using the IPluginProvider configured in the assembly\n");

                            Console.WriteLine("import filepath=[string]");
                            Console.WriteLine("  Imports a flow from file\n");

                            Console.WriteLine("flows - list the available flows\n");
                            Console.WriteLine("functions - list the available functions\n");
                            Console.WriteLine("plugins - list the available plugins\n");
                            Console.WriteLine("help - display available command formats\n");
                            Console.WriteLine("exit - Stop all threads and exit the agent\n");
                            Console.WriteLine("=============================");
                        }, _cancellationToken);
                    }
                }
            };
        }

        private void WriteWelcome()
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to Runic Agent");
            Console.WriteLine("Please enter a command. type 'help' for options");
        }

        private async Task LoadPlugin(string pluginKey, CancellationToken ct)
        {
            await Task.Run(() => _pluginManager.LoadPlugin(pluginKey));
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
