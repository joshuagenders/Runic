using System.Collections.Generic;

namespace Runic.Agent.Core
{
    public interface IPersonAttributeService
    {
        Dictionary<string,string> RequestAttributes();
        void ReleaseAttributes(Dictionary<string, string> attributes);
    }
}