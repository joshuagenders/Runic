namespace Runic.Agent.Worker.Test.TestUtility
{
    public class TestConstants
    {
        public static readonly string[] CommandLineArguments = new string[]
            {
                "Agent:MaxThreads=321",
                "Agent:LifetimeSeconds=123",
                "Client:MQConnectionString=MyExampleConnection",
                "Statsd:Port=8125",
                "Statsd:Host=192.168.99.100",
                "Statsd:Prefix=Runic.Stats."
            };
        public const string AssemblyName = "Runic.ExampleTest";
        public const string FunctionName = "FakeFunction";
        public const string AssemblyQualifiedClassName = "Runic.ExampleTest.Functions.FakeFunction";
    }
}
