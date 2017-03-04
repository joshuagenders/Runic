using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Attributes;

namespace Runic.Agent.Harness
{
    public class FunctionHarness<T>
    {
        public FunctionHarness(T instance)
        {
            _instance = instance;
        }

        private T _instance { get; set; }
        
        public async Task Execute(CancellationToken ct, string functionName)
        {
            await BeforeEach()
                    .ContinueWith((_) => ExecuteFunction(functionName))
                    .ContinueWith((_) => AfterEach());
        }

        private async Task ExecuteFunction(string name)
        {
            var methods = _instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<FunctionAttribute>();
                if (attribute != null && attribute.Name == name)
                    await Task.Run(() => method.Invoke(_instance, null));
            }
        }

        private async Task ExecuteMethodWithAttribute(Type attributeType)
        {
            var methods = _instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute(attributeType);
                if (attribute != null)
                    await Task.Run(() => method.Invoke(_instance, null));
            }
        }

        private async Task BeforeEach()
        {
            await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute));
        }

        private async Task AfterEach()
        {
            await ExecuteMethodWithAttribute(typeof(AfterEachAttribute));
        }

        private async Task TeardownFunctionClass()
        {
            await ExecuteMethodWithAttribute(typeof(ClassTeardownAttribute));
        }
    }
}