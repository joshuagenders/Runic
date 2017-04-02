using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Data
{
    public class DataService : IDataService
    {
        private List<IDataProvider> _providers { get; set; }

        public bool CanShareRunesAcrossNetwork { get; set; }

        public DataService(IList<IDataProvider> providers)
        {
            _providers = providers.ToList();
        }

        public object[] GetParameters(string datasourceId, Dictionary<string,string> datasourceMapping)
        {
            return null;
        }
    }
}
