using Runic.Agent.Core.Models;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.Core.AssemblyManagement
{
    public interface IAssemblyManager
    {
        IList<MethodStepInformation> GetAvailableMethods();
        void LoadAssembly(string pluginAssemblyName);
        Assembly GetAssembly(string pluginAssemblyName);
    }
}
