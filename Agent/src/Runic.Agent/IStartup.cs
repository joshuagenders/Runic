using Autofac;

namespace Runic.Agent
{
    public interface IStartup
    {
        IContainer BuildContainer(string [] args = null);
    }
}