﻿using Runic.Agent.Core.ExternalInterfaces;
using System.Collections.Generic;

namespace Runic.Agent.Standalone.Services
{
    public class NoOpDataService : IDataService
    {
        public object[] GetMethodParameterValues(string datasourceId, Dictionary<string, string> datasourceMapping)
        {
            return new object[] { };
        }
    }
}
