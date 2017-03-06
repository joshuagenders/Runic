using Autofac;

namespace Runic.Agent
{
    public static class IoC
    {
        public static IContainer Container { get; set; }
        public static void RegisterDependencies(IStartup startup)
        {
            Container = startup.RegisterDependencies();
        }
    }
}
