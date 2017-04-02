using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.FlowManagement
{
    public interface IFlowManager
    {
        void AddUpdateFlow(Flow flow);
        Flow GetFlow(string name);
        IList<Flow> GetFlows();
    }
}
