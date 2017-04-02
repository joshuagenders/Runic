using System;
using System.Collections.Generic;

namespace Runic.Agent.Data
{
    public class DataService : IDataService
    {
        public bool CanShareRunesAcrossNetwork { get; set; }
        public Dictionary<string, Func<string, string, object[]>> handlers { get; set; }
        public DataService()
        {
            //todo add different handlers with priority order
        }

        public object[] GetParameters(string datasourceId, Dictionary<string,string> datasourceMapping)
        {
            return null;
        }
    }
}
