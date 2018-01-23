using System.Reflection;

namespace Runic.Cucumber
{
    public interface IAssemblyAdapter
    {
        void ExecuteMethod(object instance, MethodInfo method, object[] arguments);
        void ExecuteMethodFromStatement(string statement, object[] arguments);
    }
}
