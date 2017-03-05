using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Attributes;

namespace Runic.Agent.Harness
{
    public class FunctionHarness : IFunctionHarness
    {
        private object _instance { get; set; }

        public void Bind(object functionInstance)
        {
            _instance = functionInstance;
        }
        
        public async Task Execute(string functionName, CancellationToken ct)
        {
            await BeforeEach();
            await ExecuteFunction(functionName);
            await AfterEach();
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
    }
}