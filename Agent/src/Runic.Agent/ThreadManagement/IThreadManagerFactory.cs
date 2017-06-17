namespace Runic.Agent.ThreadManagement
{
    public interface IThreadManagerFactory
    {
        ThreadManager Get(string flow);
    }
}
