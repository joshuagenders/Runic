using System.Reflection;

namespace Runic.Agent.TestHarness.Services
{
    public interface IDataService
    {
        object[] GetParams(string[] positionalParameters, MethodInfo methodInfo);
    }
}
