using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Attributes;

namespace Runic.Agent.Harness
{
    public class TestHarness
    {
        public TestHarness(Type testType)
        {
            TestType = testType;
        }

        private object Instance { get; set; }
        private Type TestType { get; }

        public async Task Execute(CancellationToken ct)
        {
            try
            {
                await InitialiseFunctionClass();
                while (!ct.IsCancellationRequested)
                {
                    await BeforeEach();
                    await ExecuteFunction();
                    await AfterEach();
                }
            }
            finally
            {
                await TeardownFunctionClass();
            }
        }

        private Task ExecuteFunction()
        {
            throw new NotImplementedException();
        }

        private async Task InitialiseFunctionClass()
        {
            Instance = Activator.CreateInstance(TestType, false);
            await ExecuteMethodWithAttribute(typeof(ClassInitialiseAttribute));
        }

        private async Task ExecuteMethodWithAttribute(Type attributeType)
        {
            var methods = Instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute(attributeType);
                if (attribute != null)
                    await Task.Run(() => method.Invoke(Instance, null));
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