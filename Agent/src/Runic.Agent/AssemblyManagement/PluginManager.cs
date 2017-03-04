using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using NLog;

namespace Runic.Agent.AssemblyManagement
{
    public class PluginManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly ConcurrentBag<Assembly> TestPlugins = new ConcurrentBag<Assembly>();
        private static readonly List<string> KeyNames = new List<string>();
        
        public static void LoadPlugin(string pluginAssemblyName, IPluginProvider provider = null)
        {
            if (provider == null)
                provider = Program.Container.Resolve<IPluginProvider>();

            _logger.Info($"Loading plugin {pluginAssemblyName}");
            lock (KeyNames)
            {
                if (KeyNames.Contains(pluginAssemblyName))
                    return;
            }

            provider.RetrieveSourceDll(pluginAssemblyName);
            var pluginPath = provider.GetFilepath(pluginAssemblyName);
            var loader = new AssemblyLoader(Path.GetDirectoryName(pluginPath));
            lock (TestPlugins)
            {
                TestPlugins.Add(loader.LoadFromAssemblyName(new AssemblyName(pluginAssemblyName)));
            }
            lock (KeyNames)
            {
                KeyNames.Add(pluginAssemblyName);
            }
        }

        public static Type GetTestType(string testName)
        {
            lock (TestPlugins)
            {
                foreach (var assembly in TestPlugins)
                {
                    var types = assembly.GetTypes().Where(t => t.FullName == testName);
                    var enumerable = types as Type[] ?? types.ToArray();
                    if (enumerable.Any())
                        return enumerable.First();
                }
            }

            throw new TestNotFoundInAssemblyException();
        }
    }
}