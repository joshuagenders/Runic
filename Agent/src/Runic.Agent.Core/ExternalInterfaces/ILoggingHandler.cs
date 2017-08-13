namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface ILoggingHandler
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);

        void Debug(string message, object obj);
        void Info(string message, object obj);
        void Warning(string message, object obj);
        void Error(string message, object obj);
    }
}
