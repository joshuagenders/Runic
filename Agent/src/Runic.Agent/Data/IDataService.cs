using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.Data
{
    public interface IDataService
    {
        object[] GetParameters(string datasourceId, Dictionary<string, string> datasourceMapping);
    }
}
