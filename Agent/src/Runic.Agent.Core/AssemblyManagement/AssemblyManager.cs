using Runic.Agent.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Runic.Agent.Core.Configuration;
using System;

namespace Runic.Agent.Core.AssemblyManagement
{
    public class AssemblyManager : IAssemblyManager
    {
        private readonly ConcurrentBag<Assembly> _assemblies;
        private readonly ConcurrentDictionary<string, bool> _assembliesLoaded;
        private readonly ICoreConfiguration _config;

        public AssemblyManager(ICoreConfiguration config)
        {
            _assemblies = new ConcurrentBag<Assembly>();
            _assembliesLoaded = new ConcurrentDictionary<string, bool>();
            _config = config;
        }

        public void LoadAssembly(string pluginAssemblyName)
        {
            lock (_assembliesLoaded)
            {
                if (_assembliesLoaded.ContainsKey(pluginAssemblyName))
                    return;
                _assembliesLoaded[pluginAssemblyName] = true;
            }
            try
            {
                var pluginPath = Path.Combine(_config.PluginFolderPath, pluginAssemblyName);
                if (!File.Exists(pluginPath))
                {
                    throw new AssemblyNotFoundException($"Could not find file {pluginPath}");
                }

                //load
                Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
                if (assembly == null)
                {
                    throw new AssemblyLoadException($"Could not load assembly {pluginPath}, {pluginAssemblyName}");
                }
                _assemblies.Add(assembly);
            }
            catch
            {
                //failed to load
                lock (_assembliesLoaded)
                {
                    _assembliesLoaded[pluginAssemblyName] = false;
                }
                throw;
            }
        }

        public IList<MethodInformation> GetAvailableMethods() =>
            _assemblies.SelectMany(a => SelectMethodInformation(a.DefinedTypes)).ToList();
        
        private IEnumerable<MethodInformation> SelectMethodInformation(IEnumerable<TypeInfo> types) => 
                types.SelectMany(
                        t => t.AsType()
                              .GetRuntimeMethods()
                              .Select(SelectMethodInformation));

        private MethodInformation SelectMethodInformation (MethodInfo methodInfo) => new MethodInformation()
        {
            AssemblyName = methodInfo.DeclaringType.DeclaringType.GetTypeInfo().Assembly.FullName,
            AssemblyQualifiedClassName = methodInfo.DeclaringType.FullName,
            MethodName = methodInfo.Name
        };

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
            return _assembliesLoaded.Keys.ToList();
        }

        public Assembly GetAssembly(string pluginAssemblyName)
        {
            if (_assembliesLoaded.TryGetValue(pluginAssemblyName, out bool loaded) && loaded)
            {
                return GetAssemblies().Single(a => a.FullName == pluginAssemblyName);
            }
            throw new AssemblyNotFoundException($"Unable to locate assembly by key {pluginAssemblyName}");
        }
    }
}