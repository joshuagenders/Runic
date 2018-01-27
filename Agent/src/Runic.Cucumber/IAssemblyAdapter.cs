using System.Reflection;
using System.Threading.Tasks;

namespace Runic.Cucumber
{
    public interface IAssemblyAdapter
    {
        void ExecuteMethod(object instance, MethodInfo method, object[] arguments);
        void ExecuteMethodFromStatement(string statement, object[] arguments);
        Task ExecuteMethodFromStatementAsync(string statement, object[] arguments);
    }
}
