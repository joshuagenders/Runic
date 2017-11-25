using Runic.Agent.Core.Models;

namespace Runic.Agent.Core.Harness
{
    public interface IFunctionFactory
    {
        FunctionHarness CreateFunction(Step step, TestContext testContext);
    }
}
