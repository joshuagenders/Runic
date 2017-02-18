using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.AssemblyManagement
{
    public class PluginManager
    {
        private static readonly ConcurrentBag<Assembly> testPlugins = new ConcurrentBag<Assembly>();
        private static volatile List<string> keyNames = new List<string>();
        
        public void LoadPlugin(string pluginAssemblyName, IPluginProvider provider)
        {
            if (keyNames.Contains(pluginAssemblyName))
            {
                return;
            }

            provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = provider.GetFilepath(pluginAssemblyName);
            var loader = new AssemblyLoader(Path.GetDirectoryName(pluginPath));
            lock (testPlugins)
            {
                testPlugins.Add(loader.LoadFromAssemblyName(new AssemblyName(pluginAssemblyName)));
            }
            lock (keyNames)
            {
                keyNames.Add(pluginAssemblyName);
            }
        }
      
        public static Type GetTestType(string testName)
        {
            foreach (var assembly in testPlugins)
            {
                var types = assembly.GetTypes().Where(t => t.FullName == testName);
                if (types.Any())
                {
                    return types.First();
                }
            }

            throw new TestNotFoundInAssemblyException();
        }
    }
}
