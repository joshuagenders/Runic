using System.Collections.Generic;

namespace Runic.Agent.Core
{
    public interface IPersonAttributeService
    {
        Dictionary<string,string> RequestAttributes(string key);
        void ReleaseAttributes(string key);
    }
}