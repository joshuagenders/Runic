using Runic.Framework.Models;

namespace Runic.Agent.Core.FunctionHarness
{
    public interface IFunctionFactory
    {
        FunctionHarness CreateFunction(Step step, TestContext testContext);
    }
}
