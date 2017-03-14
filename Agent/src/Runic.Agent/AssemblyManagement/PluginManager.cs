using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using NLog;
using StatsN;
using Runic.Framework.Clients;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using Runic.Agent.Metrics;

namespace Runic.Agent.AssemblyManagement
{
    public static class PluginManager
    {
        private static readonly Logger _logger = LogManager.GetLogger("Runic.Agent.AssemblyManagement.PluginManager");
        private static ConcurrentBag<Assembly> _assemblies = new ConcurrentBag<Assembly>();
        private static ConcurrentDictionary<string, bool> _assembliesLoaded = new ConcurrentDictionary<string, bool>();
        private static IRuneClient _runeClient { get; set; }


        public static List<Assembly> GetAssemblies()
        {
            List<Assembly> assemblyList;
            lock (_assemblies)
            {
                assemblyList = _assemblies.ToList();
            }
            return assemblyList;
        }

        public static void RegisterRuneClient(IRuneClient runeClient)
        {
            _runeClient = runeClient;
        }

        public static void LoadPlugin(string pluginAssemblyName, IPluginProvider provider)
        {
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
            _logger.Debug($"Plugin path {pluginPath}");

            Assembly assembly = LoadAssembly(pluginPath, pluginAssemblyName);
            
            PopulateStaticInterfaces(assembly);

            Clients.Statsd?.Count("plugins.pluginLoaded");
        }

        public static Assembly LoadAssembly(string pluginPath, string pluginAssemblyName)
        {
            Assembly assembly = null;
            lock (_assemblies)
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
                _assemblies.Add(assembly);
            }
            lock (_assembliesLoaded)
            {
                _assembliesLoaded[pluginAssemblyName] = true;
            }
            return assembly;
            
            //foreach (var refAssembly in assembly.GetReferencedAssemblies())
            //{
            //    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
            //}
        }

        public static void ClearAssemblies()
        {
            _assembliesLoaded = new ConcurrentDictionary<string, bool>();
            _assemblies = new ConcurrentBag<Assembly>();
        }

        public static List<FunctionInformation> GetAvailableFunctions()
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
                            // todo add params and runes
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

        private static void PopulateStaticInterfaces(Assembly assembly)
        {
            //todo fix
            foreach (var type in assembly.DefinedTypes)
            {
                var staticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                staticFields.Where(f => f.GetType() == typeof(IStatsd))
                            .ToList()
                            .ForEach(f => f.SetValue(type, Clients.Statsd));
                staticFields.Where(f => f.GetType() == typeof(IRuneClient))
                            .ToList()
                            .ForEach(f => f.SetValue(type, _runeClient));
            }
        }

        public static Type GetClassType(string className)
        {
            lock (_assemblies)
            {
                foreach (var assembly in _assemblies)
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
            lock (_assemblies)
            {
                foreach (var assembly in _assemblies)
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