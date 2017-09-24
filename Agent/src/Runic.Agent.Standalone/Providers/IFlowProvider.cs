using Runic.Agent.Framework.Models;

namespace Runic.Agent.Standalone.Providers
{
    public interface IFlowProvider
    {
        Journey GetFlow(string key);
    }
}
