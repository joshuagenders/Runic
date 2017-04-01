using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.AssemblyManagement
{
    public interface IPluginManager
    {
        List<Assembly> GetAssemblies();
        List<string> GetAssemblyKeys();
        void LoadPlugin(string pluginAssemblyName);
        Assembly LoadAssembly(string pluginPath, string pluginAssemblyName);
        List<FunctionInformation> GetAvailableFunctions();
        Type GetClassType(string functionFullyQualifiedName);
    }
}
