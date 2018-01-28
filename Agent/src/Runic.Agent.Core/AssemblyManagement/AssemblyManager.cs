using Runic.Agent.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Runic.Agent.Core.AssemblyManagement
{
    public class AssemblyManager
    {
        private readonly Dictionary<string, Assembly> _assemblies;
        
        public AssemblyManager()
        {
            _assemblies = new Dictionary<string, Assembly>();
        }

        public void LoadAssembly(string pluginAssemblyPath)
        {
            if (_assemblies.ContainsKey(pluginAssemblyPath))
                return;
            ;
            if (!File.Exists(pluginAssemblyPath))
            {
                throw new ArgumentException($"Could not find file {pluginAssemblyPath}");
            }
            
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginAssemblyPath);
            _assemblies[pluginAssemblyPath] = assembly ?? throw new ArgumentException($"Could not load assembly {pluginAssemblyPath}");
        }

        public IList<Assembly> GetAssemblies() => _assemblies.Values.ToList();
        public IList<string> GetAssemblyKeys() => _assemblies.Keys.ToList();

        public IList<MethodStepInformation> GetAvailableMethods() =>
            _assemblies.SelectMany(a => MapMethodInformation(a.Value.DefinedTypes))
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

        public Assembly GetAssembly(string pluginAssemblyPath)
        {
            if (_assemblies.TryGetValue(pluginAssemblyPath, out Assembly val))
            {
                return val;
            }
            throw new ArgumentException($"Unable to locate assembly by key {pluginAssemblyPath}");
        }
    }
}