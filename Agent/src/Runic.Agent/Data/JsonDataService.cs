using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Runic.Agent.Data
{
    public class JsonDataService : IDataService
    {
        public bool CanShareRunesAcrossNetwork { get; set; }

        public JsonDataService()
        {
        }
        
        public T GetMessage<T>(string messageType)
        {
            return default(T);
            //JsonConvert.DeserializeObject<T>
        }

        public object[] GetParameters(string datasourceId, Dictionary<string, string> datasourceMapping)
        {
            throw new NotImplementedException();
        }
    }
}
