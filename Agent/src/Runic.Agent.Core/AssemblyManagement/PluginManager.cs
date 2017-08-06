using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Runic.Framework.Clients;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using Runic.Agent.Core.Metrics;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Runic.Agent.Core.AssemblyManagement
{
    public class PluginManager : IPluginManager
    {
        private readonly ConcurrentBag<Assembly> _assemblies;
        private readonly ConcurrentDictionary<string, bool> _assembliesLoaded;
        private readonly ILogger _logger;
        private readonly IStatsClient _stats;
        private readonly IRuneClient _runeClient;
        private readonly IPluginProvider _provider;

        public PluginManager(IRuneClient client, IPluginProvider provider, IStatsClient stats, ILoggerFactory loggerFactory)
        {
            _assemblies = new ConcurrentBag<Assembly>();
            _assembliesLoaded = new ConcurrentDictionary<string, bool>();
            _runeClient = client;
            _provider = provider;
            _stats = stats;
            _logger = loggerFactory.CreateLogger<PluginManager>();
        }

        public void LoadPlugin(string pluginAssemblyName)
        {
            _logger.LogDebug($"Loading plugin {pluginAssemblyName}");
            bool loaded;
            lock (_assembliesLoaded)
            {
                _assembliesLoaded.TryGetValue(pluginAssemblyName, out loaded);
            }
            if (loaded)
                return;

            _provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = _provider.GetFilepath(pluginAssemblyName);
            _logger.LogDebug($"Plugin path {pluginPath}");
            if (!File.Exists(pluginPath))
            {
                _logger.LogError($"Could not find file {pluginPath}");
                throw new AssemblyNotFoundException($"Could not find file {pluginPath}");
            }

            Assembly assembly = LoadAssembly(pluginPath, pluginAssemblyName);

            if (assembly == null)
                throw new AssemblyLoadException($"Could not load assembly {pluginPath}, {pluginAssemblyName}");

            assembly.PopulateStaticPropertiesWithInstance(_runeClient)
                    .PopulateStaticPropertiesWithInstance(_stats);

            _stats.CountPluginLoaded();
        }

        private Assembly LoadAssembly(string pluginPath, string pluginAssemblyName)
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

        public Type GetClassType(string functionFullyQualifiedName)
        {
            _logger.LogDebug($"Searching assemblies for function");
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

        public IList<FunctionInformation> GetAvailableFunctions()
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
                                                    .ToDictionary(p => p.Name, p => p.ParameterType),
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

        public IList<Assembly> GetAssemblies()
        {
            List<Assembly> assemblyList;
            lock (_assemblies)
            {
                assemblyList = _assemblies.ToList();
            }
            return assemblyList;
        }

        public IList<string> GetAssemblyKeys()
        {
            return _assembliesLoaded.Where(t => t.Value).Select(t => t.Key).ToList();
        }

        public Assembly GetPlugin(string pluginAssemblyName)
        {
            bool loaded;
            if (_assembliesLoaded.TryGetValue(pluginAssemblyName, out loaded) && loaded)
            {
                return GetAssemblies().Single(a => a.FullName == pluginAssemblyName);
            }
            throw new AssemblyNotFoundException($"Unable to locate assembly by key {pluginAssemblyName}");
        }
    }
}