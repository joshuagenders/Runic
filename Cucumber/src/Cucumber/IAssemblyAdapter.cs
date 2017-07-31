using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber
{
    public interface IAssemblyAdapter
    {
        Task ExecuteMethodAsync(object instance, MethodInfo method, CancellationToken ctx, params object[] inputParams);
        Task ExecuteMethodFromStatementAsync(string statement, object[] arguments, CancellationToken ctx = default(CancellationToken));
    }
}
