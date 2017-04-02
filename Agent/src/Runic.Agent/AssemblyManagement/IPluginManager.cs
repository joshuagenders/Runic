using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.AssemblyManagement
{
    public interface IPluginManager
    {
        IList<Assembly> GetAssemblies();
        IList<string> GetAssemblyKeys();
        IList<FunctionInformation> GetAvailableFunctions();
        Type GetClassType(string functionFullyQualifiedName);
        Assembly LoadAssembly(string pluginPath, string pluginAssemblyName);
        void LoadPlugin(string pluginAssemblyName);
    }
}
