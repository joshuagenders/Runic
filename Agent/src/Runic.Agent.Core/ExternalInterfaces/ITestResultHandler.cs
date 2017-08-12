using Runic.Agent.Core.CucumberHarness;
using Runic.Agent.Core.FunctionHarness;
using Runic.Framework.Models;

namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface ITestResultHandler
    {
        void OnFunctionComplete(FunctionResult result);
        void OnCucumberComplete(CucumberResult result);
        void OnFlowStart(Flow flow);
        void OnFlowComplete(Flow flow);
        void OnThreadChange(string flowId, int threadCount);
    }
}
