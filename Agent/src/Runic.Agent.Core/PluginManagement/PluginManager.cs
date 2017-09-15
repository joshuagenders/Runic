using Runic.Agent.Framework.ExternalInterfaces;
using Runic.Agent.Core.Services;
using Runic.Agent.Framework.Clients;
using Runic.Agent.Framework.Attributes;
using Runic.Agent.Framework.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Runic.Agent.Core.PluginManagement
{
    public class PluginManager : IPluginManager
    {
        private readonly ConcurrentBag<Assembly> _assemblies;
        private readonly ConcurrentDictionary<string, bool> _assembliesLoaded;
        private readonly IRuneClient _runeClient;
        private readonly IPluginProvider _provider;
        private readonly IEventService _eventService;

        public PluginManager(IRuneClient client, IPluginProvider provider, IEventService eventService)
        {
            _assemblies = new ConcurrentBag<Assembly>();
            _assembliesLoaded = new ConcurrentDictionary<string, bool>();
            _runeClient = client;
            _provider = provider;
            _eventService = eventService;
        }

        public void LoadPlugin(string pluginAssemblyName)
        {
            _eventService.Debug($"Loading plugin {pluginAssemblyName}");
            bool loaded;
            lock (_assembliesLoaded)
            {
                _assembliesLoaded.TryGetValue(pluginAssemblyName, out loaded);
            }
            if (loaded)
                return;

            _provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = _provider.GetFilepath(pluginAssemblyName);
            _eventService.Debug($"Plugin path {pluginPath}");
            if (!File.Exists(pluginPath))
            {
                _eventService.Error($"Could not find file {pluginPath}");
                throw new AssemblyNotFoundException($"Could not find file {pluginPath}");
            }

            Assembly assembly = LoadAssembly(pluginPath, pluginAssemblyName);

            if (assembly == null)
            {
                _eventService.Error($"Could not load assembly {pluginPath}, {pluginAssemblyName}");
                throw new AssemblyLoadException($"Could not load assembly {pluginPath}, {pluginAssemblyName}");
            }
            assembly.PopulateStaticPropertiesWithInstance(_runeClient);
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
                                //todo methodparams
                                RequiredRunes = method.GetCustomAttributes<RequiresRunesAttribute>()
                                                     ?.SelectMany(s => s.GetRunes())
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
            if (_assembliesLoaded.TryGetValue(pluginAssemblyName, out bool loaded) && loaded)
            {
                return GetAssemblies().Single(a => a.FullName == pluginAssemblyName);
            }
            _eventService.Error($"Unable to locate assembly by key {pluginAssemblyName}");
            throw new AssemblyNotFoundException($"Unable to locate assembly by key {pluginAssemblyName}");
        }
    }
}