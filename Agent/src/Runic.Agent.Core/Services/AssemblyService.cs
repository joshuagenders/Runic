using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Runic.Agent.Core.Services
{
    public class AssemblyService
    {
        //TODO: find a way to utilise the cache across consumers in akka
        private readonly Dictionary<string, Assembly> _assemblyCache;
        
        public AssemblyService()
        {
            _assemblyCache = new Dictionary<string, Assembly>();
        }

        private void LoadAssembly(string pluginAssemblyPath)
        {
            if (_assemblyCache.ContainsKey(pluginAssemblyPath))
                return;

            if (!File.Exists(pluginAssemblyPath))
            {
                throw new ArgumentException($"Could not find file {pluginAssemblyPath}");
            }
            
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginAssemblyPath);
            _assemblyCache[pluginAssemblyPath] = assembly ?? throw new ArgumentException($"Could not load assembly {pluginAssemblyPath}");
        }

        public virtual Assembly GetLoadAssembly(string pluginAssemblyPath)
        {
            LoadAssembly(pluginAssemblyPath);
            if (_assemblyCache.TryGetValue(pluginAssemblyPath, out Assembly val))
            {
                return val;
            }
            throw new ArgumentException($"Unable to locate assembly by key {pluginAssemblyPath}");
        }
    }
}