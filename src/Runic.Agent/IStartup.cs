using Autofac;

namespace Runic.Agent
{
    public interface IStartup
    {
        void ConfigureApplication();
        IContainer RegisterDependencies();
    }
}