using Runic.Agent.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Runic.Agent.Core.AssemblyManagement
{
    public interface IAssemblyManager
    {
        Assembly GetLoadAssembly(string pluginAssemblyPath);
    }

    public class AssemblyManager : IAssemblyManager
    {
        private readonly Dictionary<string, Assembly> _assemblyCache;
        
        public AssemblyManager()
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

        public IList<Assembly> GetCachedAssemblies() => _assemblyCache.Values.ToList();
        public IList<string> GetCachedAssemblyKeys() => _assemblyCache.Keys.ToList();

        public IList<MethodStepInformation> GetAvailableMethods() =>
            _assemblyCache
                .SelectMany(a => MapMethodInformation(a.Value.DefinedTypes))
                .ToList();
        
        private IEnumerable<MethodStepInformation> MapMethodInformation(IEnumerable<TypeInfo> types) => 
                types.SelectMany(
                        t => t.AsType()
                              .GetRuntimeMethods()
                              .Select(MapMethodInformation));

        private MethodStepInformation MapMethodInformation (
            MethodInfo methodInfo) => 
                new MethodStepInformation(
                    methodInfo.DeclaringType.DeclaringType.GetTypeInfo().Assembly.FullName,
                    methodInfo.DeclaringType.FullName,
                    methodInfo.Name
                );

        public Assembly GetLoadAssembly(string pluginAssemblyPath)
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