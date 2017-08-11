using Moq;
using System.Reflection;

namespace Runic.Cucumber.UnitTest.TestUtility
{
    public class TestEnvironment
    {
        public TestObject<IAssemblyAdapter> AssemblyAdapter { get; set; } = new TestObject<IAssemblyAdapter>();
        public TestObject<TestStateManager> TestStateManager { get; set; } = new TestObject<TestStateManager>();

        public void SetupMocks(Mock<FakeCucumberClass> test)
        {
            TestStateManager.MockObject
                            .Setup(m => m.GetObject(typeof(FakeCucumberClass)))
                            .Returns(test.Object);

            AssemblyAdapter.Instance = new AssemblyAdapter(GetType().GetTypeInfo().Assembly, TestStateManager.MockObject.Object);
        }
    }
    public static class TestEnvironmentExtensions
    {
        public static TestEnvironment WithTestStateManager(this TestEnvironment env, TestStateManager value)
        {
            env.TestStateManager.Instance = value;
            return env;
        }
        public static TestEnvironment WithAssemblyAdapter(this TestEnvironment env, IAssemblyAdapter value)
        {
            env.AssemblyAdapter.Instance = value;
            return env;
        }
    }
}
