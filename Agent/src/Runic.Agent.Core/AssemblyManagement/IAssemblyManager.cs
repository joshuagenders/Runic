using Runic.Agent.Framework.Models;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.Core.AssemblyManagement
{
    public interface IAssemblyManager
    {
        IList<FunctionInformation> GetAvailableFunctions();
        void LoadAssembly(string pluginAssemblyName);
        Assembly GetAssembly(string pluginAssemblyName);
    }
}
