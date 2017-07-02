using Autofac;

namespace Runic.Agent.Standalone
{
    public interface IStartup
    {
        IContainer BuildContainer(string[] args = null);
    }
}
