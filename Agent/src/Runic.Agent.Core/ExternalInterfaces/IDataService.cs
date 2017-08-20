﻿using System.Collections.Generic;

namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface IDataService
    {
        object[] GetMethodParameterValues(string datasourceId, Dictionary<string, string> datasourceMapping);
    }
}