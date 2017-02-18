using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using Runic.Core.Attributes;

namespace Runic.Agent
{
    public class TestHarness
    {
        private object _instance { get; set; }
        private Type _testType { get; set; }
        private string _testName { get; set; }
        private Stopwatch _testWatch { get; set; }
        private Stopwatch _stepWatch { get; set; }
        private string _uniqueTestRunId { get; set; }

        public TestHarness(Type testType)
        {
            _testType = testType;
        }

        public async Task Execute()
        {
            await InitialiseTestClass();
            //TODO execute
            await TeardownTest();
            await TeardownClass();
        }

        public async Task InitialiseTestClass()
        {
            _instance = Activator.CreateInstance(_testType, false);
            await ExecuteMethodWithAttribute(typeof(ClassInitialiseAttribute));   
        }

        private async Task ExecuteMethodWithAttribute(Type attributeType)
        {
            var methods = _instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    await Task.Run(() => method.Invoke(_instance, null));
                }
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
