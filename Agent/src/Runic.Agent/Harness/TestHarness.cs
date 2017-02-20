using System;
using System.Reflection;
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

        public async Task Execute()
        {
            await InitialiseTestClass();
            //TODO execute
            await TeardownTest();
            await TeardownClass();
        }

        public async Task InitialiseTestClass()
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

        public async Task TeardownTest()
        {
            await ExecuteMethodWithAttribute(typeof(TestTeardownAttribute));
        }

        public async Task TeardownClass()
        {
            await ExecuteMethodWithAttribute(typeof(ClassTeardownAttribute));
        }
    }
}