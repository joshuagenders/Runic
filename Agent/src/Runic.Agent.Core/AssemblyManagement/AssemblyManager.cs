using Runic.Agent.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Runic.Agent.Core.AssemblyManagement
{
    public class AssemblyManager : IAssemblyManager
    {
        private readonly Dictionary<string, Assembly> _assemblies;
        private readonly string _pluginPath;

        public AssemblyManager(string pluginPath)
        {
            _assemblies = new Dictionary<string, Assembly>();
            _pluginPath = pluginPath;
        }

        public void LoadAssembly(string pluginAssemblyName)
        {
            if (_assemblies.ContainsKey(pluginAssemblyName))
                return;
            
            var pluginPath = Path.Combine(_pluginPath, pluginAssemblyName);
            if (!File.Exists(pluginPath))
            {
                throw new AssemblyLoadException($"Could not find file {pluginPath}");
            }
            
            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
            _assemblies[pluginAssemblyName] = assembly ?? throw new AssemblyLoadException($"Could not load assembly {pluginPath}, {pluginAssemblyName}");
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

        public Assembly GetAssembly(string pluginAssemblyName)
        {
            if (_assemblies.TryGetValue(pluginAssemblyName, out Assembly val))
            {
                return val;
            }
            throw new AssemblyLoadException($"Unable to locate assembly by key {pluginAssemblyName}");
        }
    }
}