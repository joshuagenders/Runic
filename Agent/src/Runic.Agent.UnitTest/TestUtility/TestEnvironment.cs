using Autofac;
using System;

namespace Runic.Agent.UnitTest.TestUtility
{
    public class TestEnvironment : IDisposable
    {
        private ILifetimeScope lifetimeScope { get; set; }
        public readonly TestApplication App;

        public void Dispose()
        {
            lifetimeScope.Dispose();
        }

        public TestEnvironment()
        {
            var testStartup = new TestStartup();
            var container = testStartup.BuildContainer(TestConstants.CommandLineArguments);
            lifetimeScope = container.BeginLifetimeScope();
            App = lifetimeScope.Resolve<IApplication>() as TestApplication;
        }        
    }
}
