using Runic.Framework.Models;

namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface IAgentObserver
    {
        void Update(FlowInformation flowInformation);
        void Update(PatternInformation patternInformation);
    }
}
