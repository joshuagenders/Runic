//using Autofac;
using Runic.Agent.Core.UnitTest.TestUtility;
using System;

namespace Runic.Agent.Core.UnitTest.TestUtility
{
    public class TestEnvironment : IDisposable
    {
        //private ILifetimeScope lifetimeScope { get; set; }
        public readonly FakeApplication App;

        public void Dispose()
        {
           // lifetimeScope.Dispose();
        }

        public TestEnvironment()
        {
           // var testStartup = new FakeStartup();
           // var container = testStartup.BuildContainer(TestConstants.CommandLineArguments);
           // lifetimeScope = container.BeginLifetimeScope();
           // //todo fix test architecture
           // App = lifetimeScope.Resolve<IApplication>() as FakeApplication;
        }        
    }
}
