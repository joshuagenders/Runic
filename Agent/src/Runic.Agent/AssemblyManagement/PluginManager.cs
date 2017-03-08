using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using NLog;
using StatsN;

namespace Runic.Agent.AssemblyManagement
{
    public static class PluginManager
    {
        private static readonly Logger _logger = LogManager.GetLogger("Runic.Agent.AssemblyManagement.PluginManager");
        private static ConcurrentBag<Assembly> _testPlugins = new ConcurrentBag<Assembly>();
        private static ConcurrentDictionary<string, bool> _assembliesLoaded = new ConcurrentDictionary<string, bool>();
        
        public static List<Assembly> GetAssemblies()
        {
            List<Assembly> assemblyList;
            lock (_testPlugins)
            {
                assemblyList = _testPlugins.ToList();
            }
            return assemblyList;
        }

        public static void LoadPlugin(string pluginAssemblyName, IPluginProvider provider = null)
        {
            //todo use a better pattern for di
            if (provider == null)
                provider = IoC.Container.Resolve<IPluginProvider>();

            _logger.Debug($"Loading plugin {pluginAssemblyName}");
            bool loaded;
            lock (_assembliesLoaded)
            {
                _assembliesLoaded.TryGetValue(pluginAssemblyName, out loaded);
            }
            if (loaded)
                return;            

            provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = provider.GetFilepath(pluginAssemblyName);

            lock (_testPlugins)
            {
                //_testPlugins.Add(loader.LoadFromAssemblyName(new AssemblyName(pluginAssemblyName)));
                _testPlugins.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath));
            }
            lock (_assembliesLoaded)
            {
                _assembliesLoaded[pluginAssemblyName] = true;
            }
            var statsd = IoC.Container?.Resolve<IStatsd>();
            statsd?.Count("pluginLoaded");
        }

        public static Type GetClassType(string className)
        {
            lock (_testPlugins)
            {
                foreach (var assembly in _testPlugins)
                {
                    var types = assembly.GetTypes().Where(t => t.FullName == className);
                    var enumerable = types as Type[] ?? types.ToArray();
                    if (enumerable.Any())
                        return enumerable.First();
                }
            }

            throw new ClassNotFoundInAssemblyException();
        }

        public static Type GetFunctionType(string functionFullyQualifiedName)
        {
            //_logger.Debug($"Searching assemblies for function");
            lock (_testPlugins)
            {
                foreach (var assembly in _testPlugins)
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