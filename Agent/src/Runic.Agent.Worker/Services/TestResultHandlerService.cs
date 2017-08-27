using Runic.Agent.Core.CucumberHarness;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FunctionHarness;
using Runic.Framework.Models;

namespace Runic.Agent.Worker.Services
{
    public class TestResultHandlerService : ITestResultHandler
    {
        public void OnCucumberComplete(CucumberResult result)
        {
        }

        public void OnFlowComplete(Flow flow)
        {
        }

        public void OnFlowStart(Flow flow)
        {
        }

        public void OnFunctionComplete(FunctionResult result)
        {
        }

        public void OnThreadChange(Flow flow, int threadCount)
        {
        }
    }
}
