namespace Runic.Agent.Harness
{
    public interface IThreadManagerFactory
    {
        ThreadManager Get(string flow);
    }
}
