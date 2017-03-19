using Autofac;

namespace Runic.Agent
{
    public interface IStartup
    {
        IContainer Register();
    }
}