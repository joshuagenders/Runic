using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Core.FlowManagement
{
    public interface IFlowManager
    {
        void AddUpdateFlow(Flow flow);
        Flow GetFlow(string name);
        IList<Flow> GetFlows();
    }
}
