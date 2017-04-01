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

namespace Runic.Agent.Console.Shell
{
    public class AgentShell
    {
        private Dictionary<string, Func<string[], Task>> _handlers { get; set; }
        private bool _return { get; set; }
        private IAgentService _agentService { get; }
        private CancellationToken _cancellationToken { get; set; }
        private readonly IPluginManager _pluginManager;
        private readonly IFlowManager _flowManager;

        public AgentShell(IAgentService agentService, IFlowManager flowManager, PluginManager pluginManager)
        {
            _return = false;
            _agentService = agentService;
            _pluginManager = pluginManager;
            _flowManager = flowManager;

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
                    string[] input = System.Console.ReadLine().Split();
                    if (!input.Any()) continue;
                    if (input[0] == "exit") return;

                    if (_handlers.ContainsKey(input[0]))
                    {
                        await _handlers[input[0]](input);
                    }
                    else
                    {
                        System.Console.WriteLine($"No handler was found for the given command '{input[0]}'");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Encountered error");
                    System.Console.WriteLine(JsonConvert.SerializeObject(e));
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
                            System.Console.WriteLine("missing parameter 'flow'");
                            return;
                        }
                        if (!vals.ContainsKey("threadcount")){
                            System.Console.WriteLine("missing parameter 'threadcount'");
                            return;
                        }

                        if (vals.ContainsKey("pluginkey"))
                        {
                            await Task.Run(() => LoadPlugin(vals["pluginkey"], _cancellationToken));
                        }

                        await SetThreadLevel(vals["flow"], int.Parse(vals["threadcount"]),_cancellationToken);
                    }
                },
                {
                    "load", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        if (!vals.ContainsKey("pluginkey")){
                            System.Console.WriteLine("missing parameter 'pluginkey'");
                            return;
                        }
                        System.Console.WriteLine($"Attempting to load plugin {vals["pluginkey"]}");
                        await Task.Run(() => LoadPlugin(vals["pluginkey"], _cancellationToken));
                    }
                },
                {
                    "functions", async (input) =>
                    {
                        await Task.Run(() => PrintFunctions());
                    }
                },
                {
                    "import", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        if (!vals.ContainsKey("filepath")){
                            System.Console.WriteLine("missing parameter 'filepath'");
                            return;
                        }
                        await Task.Run(() => ImportFlow(vals), _cancellationToken);
                    }
                },
                {
                    "plugins", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await Task.Run(() => PrintPlugins(vals), _cancellationToken);
                    }
                },
                {
                    "flows", async (input) =>
                    {
                        var vals = input.FromKeywordToDictionary();
                        await Task.Run(() => PrintFlows(vals), _cancellationToken);
                    }
                },
                {
                    "wd", async (input) =>
                    {
                        await Task.Run(() => System.Console.WriteLine(Directory.GetCurrentDirectory()), _cancellationToken);
                    }
                },
                {
                    "help", async (input) =>
                    {
                        await Task.Run(() => PrintHelp(), _cancellationToken);
                    }
                }
            };
        }

        private void WriteWelcome()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Welcome to Runic Agent");
            System.Console.WriteLine("Please enter a command. type 'help' for options");
        }

        private void ImportFlow(Dictionary<string,string> vals)
        {
            var text = File.ReadAllText(vals["filepath"]);
            var flow = JsonConvert.DeserializeObject<Flow>(text);
            _flowManager.AddUpdateFlow(flow);
        }

        private void PrintFunctions()
        {
            var functions = _pluginManager.GetAvailableFunctions();
            functions.ForEach(f => System.Console.WriteLine(JsonConvert.SerializeObject(f)));
        }

        private void PrintPlugins(Dictionary<string,string> vals)
        {
            if (vals.ContainsKey("-v"))
            {
                System.Console.WriteLine("Assemblies:");
                _pluginManager.GetAssemblies().ForEach(a => System.Console.WriteLine(a.FullName));
            }
            System.Console.WriteLine("Keys:");
            _pluginManager.GetAssemblyKeys().ForEach(a => System.Console.WriteLine(a));
        }

        private void PrintFlows(Dictionary<string,string> vals)
        {
            _flowManager.GetFlows()
                  .ForEach(f =>
                  {
                      if (vals.ContainsKey("-v"))
                          System.Console.WriteLine(JsonConvert.SerializeObject(f));
                      else
                          System.Console.WriteLine(f.Name);
                  }
                   );
        }

        private void PrintHelp()
        {
            System.Console.WriteLine("=============================");
            System.Console.WriteLine("            HELP");
            System.Console.WriteLine("=============================");
            System.Console.WriteLine("setthread pluginkey=[string] flow=[filename] threadcount=[int]");
            System.Console.WriteLine("  Sets the thread level for a flow\n");
            System.Console.WriteLine("  Starts or stops a flow when threadCount moves to and from 0\n");
            
            System.Console.WriteLine("load pluginkey=[string]");
            System.Console.WriteLine("  Loads an assembly using the IPluginProvider configured in the assembly\n");
            
            System.Console.WriteLine("import filepath=[string]");
            System.Console.WriteLine("  Imports a flow from file\n");
            
            System.Console.WriteLine("wd - display the current working directory\n");
            System.Console.WriteLine("flows - list the available flows\n");
            System.Console.WriteLine("functions - list the available functions\n");
            System.Console.WriteLine("plugins - list the available plugins\n");
            System.Console.WriteLine("help - display available command formats\n");
            System.Console.WriteLine("exit - Stop all threads and exit the agent\n");
            System.Console.WriteLine("=============================");
        }

        private void LoadPlugin(string pluginKey, CancellationToken ct)
        {
            _pluginManager.LoadPlugin(pluginKey);
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
