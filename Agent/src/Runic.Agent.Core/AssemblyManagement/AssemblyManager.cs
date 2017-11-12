using Runic.Agent.Framework.ExternalInterfaces;
using Runic.Agent.Framework.Attributes;
using Runic.Agent.Framework.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Runic.Agent.Core.AssemblyManagement
{
    public class AssemblyManager : IAssemblyManager
    {
        private readonly ConcurrentBag<Assembly> _assemblies;
        private readonly ConcurrentDictionary<string, bool> _assembliesLoaded;
        private readonly IPluginProvider _provider;
        
        public AssemblyManager(IPluginProvider provider)
        {
            _assemblies = new ConcurrentBag<Assembly>();
            _assembliesLoaded = new ConcurrentDictionary<string, bool>();
            _provider = provider;
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
                //retrieve
                _provider.RetrieveSourceDll(pluginAssemblyName);
                var pluginPath = _provider.GetFilepath(pluginAssemblyName);
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
                                FunctionName = attribute.Name
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