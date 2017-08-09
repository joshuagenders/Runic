using Runic.Agent.Core.ExternalInterfaces;
using System.Collections.Generic;

namespace Runic.Agent.Worker.Services
{
    public class MessagingDataService : IDataService
    {
        public object[] GetMethodParameterValues(string datasourceId, Dictionary<string, string> datasourceMapping)
        {
            return new object[] { };
        }
    }
}
