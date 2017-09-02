using Autofac;

namespace Runic.Agent.Aws
{
    public interface IStartup
    {
        IContainer BuildContainer(string[] args = null);
    }
}
