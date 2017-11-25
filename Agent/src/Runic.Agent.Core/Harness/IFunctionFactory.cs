using Runic.Agent.Core.Models;

namespace Runic.Agent.Harness
{
    public interface IFunctionFactory
    {
        FunctionHarness CreateFunction(Step step, TestContext testContext);
    }
}
