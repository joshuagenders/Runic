using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.CucumberHarness;
using Runic.Agent.Core.FunctionHarness;
using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Core.Services
{
    public class TestResultHandlerService : ITestResultHandler
    {
        private readonly List<ITestResultHandler> _handlers;
        public TestResultHandlerService(List<ITestResultHandler> handlers)
        {
            _handlers = handlers;
        }
        public TestResultHandlerService()
        {
            _handlers = new List<ITestResultHandler>();
        }

        public void OnCucumberComplete(CucumberResult result)
        {
            _handlers.ForEach(h => h.OnCucumberComplete(result));
        }

        public void OnFlowComplete(Flow flow)
        {
            _handlers.ForEach(h => h.OnFlowComplete(flow));
        }

        public void OnFlowStart(Flow flow)
        {
            _handlers.ForEach(h => h.OnFlowStart(flow));
        }

        public void OnFunctionComplete(FunctionResult result)
        {
            _handlers.ForEach(h => h.OnFunctionComplete(result));
        }

        public void OnThreadChange(string flowId, int threadCount)
        {
            _handlers.ForEach(h => h.OnThreadChange(flowId, threadCount));
        }
    }
}
