﻿using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Runic.Agent.Core.AssemblyManagement
{
    public interface IPluginManager
    {
        IList<FunctionInformation> GetAvailableFunctions();
        Type GetClassType(string functionFullyQualifiedName);
        void LoadPlugin(string pluginAssemblyName);
        Assembly GetPlugin(string pluginAssemblyName);
    }
}
