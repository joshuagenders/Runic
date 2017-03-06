using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using NLog;
using Runic.Core.Attributes;

namespace Runic.Agent.AssemblyManagement
{
    public class PluginManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly ConcurrentBag<Assembly> _testPlugins = new ConcurrentBag<Assembly>();
        private static readonly List<string> _keyNames = new List<string>();

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
            lock (_keyNames)
            {
                if (_keyNames.Contains(pluginAssemblyName))
                    return;
            }

            provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = provider.GetFilepath(pluginAssemblyName);
            //var loader = new AssemblyLoader(Path.GetDirectoryName(pluginPath));
            lock (_keyNames)
            {
                _keyNames.Add(pluginAssemblyName);
            }
            lock (_testPlugins)
            {
                //_testPlugins.Add(loader.LoadFromAssemblyName(new AssemblyName(pluginAssemblyName)));
                _testPlugins.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath));
            }
            
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
            _logger.Debug($"Searching assemblies for function");
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