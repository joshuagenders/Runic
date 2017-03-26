using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using NLog;
using StatsN;
using Runic.Framework.Clients;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using Runic.Agent.Metrics;
using System.IO;

namespace Runic.Agent.AssemblyManagement
{
    public class PluginManager
    {
        private readonly Logger _logger = LogManager.GetLogger("Runic.Agent.AssemblyManagement.PluginManager");
        private readonly ConcurrentBag<Assembly> _assemblies;
        private readonly ConcurrentDictionary<string, bool> _assembliesLoaded;
        private IRuneClient _runeClient { get; set; }
        private IPluginProvider _provider { get; set; }

        public PluginManager()
        {
            _assemblies = new ConcurrentBag<Assembly>();
            _assembliesLoaded = new ConcurrentDictionary<string, bool>();
        }

        public List<Assembly> GetAssemblies()
        {
            List<Assembly> assemblyList;
            lock (_assemblies)
            {
                assemblyList = _assemblies.ToList();
            }
            return assemblyList;
        }

        public List<string> GetAssemblyKeys()
        {
            return _assembliesLoaded.Where(t => t.Value).Select(t => t.Key).ToList();
        }

        public void RegisterRuneClient(IRuneClient runeClient)
        {
            _runeClient = runeClient;
        }

        public void RegisterProvider(IPluginProvider provider)
        {
            _provider = provider;
        }

        public void LoadPlugin(string pluginAssemblyName)
        {
            _logger.Debug($"Loading plugin {pluginAssemblyName}");
            bool loaded;
            lock (_assembliesLoaded)
            {
                _assembliesLoaded.TryGetValue(pluginAssemblyName, out loaded);
            }
            if (loaded)
                return;            

            _provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = _provider.GetFilepath(pluginAssemblyName);
            _logger.Debug($"Plugin path {pluginPath}");
            if (!File.Exists(pluginPath))
            {
                Console.WriteLine($"Could not find file {pluginPath}");
                throw new FileNotFoundException($"Could not find file {pluginPath}");
            }

            Assembly assembly = LoadAssembly(pluginPath, pluginAssemblyName);

            if (assembly == null)
                throw new AssemblyLoadException($"Could not load assembly {pluginPath}, {pluginAssemblyName}");

            PopulateStaticInterfaces(assembly);

            Stats.CountPluginLoaded();
        }

        public Assembly LoadAssembly(string pluginPath, string pluginAssemblyName)
        {
            Assembly assembly = null;
            lock (_assemblies)
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
                _assemblies.Add(assembly);
            }
            lock (_assembliesLoaded)
            {
                _assembliesLoaded[pluginAssemblyName] = true;
            }

            return assembly;
        }

        public List<FunctionInformation> GetAvailableFunctions()
        {
            var functions = new List<FunctionInformation>();
            foreach (var assembly in _assemblies)
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    var methods = type.AsType().GetRuntimeMethods();
                    foreach (var method in methods)
                    {
                        var attribute = method.GetCustomAttribute<FunctionAttribute>();
                        if (attribute != null)
                        {
                            functions.Add(new FunctionInformation()
                            {
                                AssemblyName = assembly.FullName,
                                AssemblyQualifiedClassName = type.FullName,
                                FunctionName = attribute.Name,
                                Parameters = method.GetParameters()
                                                    .ToDictionary(p=> p.Name, p => p.ParameterType),
                                RequiredRunes = method.GetCustomAttributes<RequiresRunesAttribute>()
                                                     ?.SelectMany(s => s.Runes)
                                                      .ToList()
                            });
                        }
                    }
                }
            }
            return functions;
        }

        private void PopulateStaticInterfaces(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                var staticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                staticFields.Where(f => f.GetType() == typeof(IStatsd))
                            .ToList()
                            .ForEach(f => f.SetValue(type, Stats.Statsd));
                staticFields.Where(f => f.GetType() == typeof(IRuneClient))
                            .ToList()
                            .ForEach(f => f.SetValue(type, _runeClient));
            }
        }

        public Type GetClassType(string functionFullyQualifiedName)
        {
            _logger.Debug($"Searching assemblies for function");
            lock (_assemblies)
            {
                foreach (var assembly in _assemblies)
                {
                    var type = assembly.GetType(functionFullyQualifiedName);
                    if (type != null)
                        return type;
                }
            }

            throw new ClassNotFoundInAssemblyException(functionFullyQualifiedName);
        }
    }
}