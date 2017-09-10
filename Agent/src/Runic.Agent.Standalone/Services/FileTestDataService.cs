using System;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Standalone.Services
{
    public class FileTestDataService : ITestDataService
    {
        public object GetMethodParameterValue(string datasourceId, string datasourceKey)
        {
            //todo
            return datasourceKey;
        }
    }
}
