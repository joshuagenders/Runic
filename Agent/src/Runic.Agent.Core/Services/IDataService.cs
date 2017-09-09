using System.Reflection;

namespace Runic.Agent.Core.Services
{
    public interface IDataService
    {
        object[] GetParams(string[] positionalParameters, MethodInfo methodInfo);
    }
}
