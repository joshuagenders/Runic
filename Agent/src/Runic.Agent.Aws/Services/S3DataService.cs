using System;
using System.Collections.Generic;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Aws.Services
{
    public class S3DataService : IDataService
    {
        public object[] GetMethodParameterValues(string datasourceId, Dictionary<string, string> datasourceMapping)
        {
            //assumes csv
            //read file from s3
            //read into memory - cache
            //map values into array to pass into method
            //make enumerable?
            throw new NotImplementedException();
        }
    }
}
