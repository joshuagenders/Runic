using Runic.Agent.Framework.Models;

namespace Runic.Agent.TestHarness.Harness
{
    public interface IFunctionFactory
    {
        FunctionHarness CreateFunction(Step step, TestContext testContext);
    }
}
