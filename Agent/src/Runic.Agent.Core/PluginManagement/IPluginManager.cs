using Runic.Agent.Framework.Models;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.Core.PluginManagement
{
    public interface IPluginManager
    {
        IList<FunctionInformation> GetAvailableFunctions();
        void LoadPlugin(string pluginAssemblyName);
        Assembly GetPlugin(string pluginAssemblyName);
    }
}
