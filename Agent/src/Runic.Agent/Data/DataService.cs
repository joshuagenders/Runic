using System.Collections.Generic;

namespace Runic.Agent.Data
{
    public class DataService : IDataService
    {
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
