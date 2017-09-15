using Runic.Agent.Framework.Models;

namespace Runic.Agent.Standalone.Providers
{
    public interface IFlowProvider
    {
        Flow GetFlow(string key);
    }
}
