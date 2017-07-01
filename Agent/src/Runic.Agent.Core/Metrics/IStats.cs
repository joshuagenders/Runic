namespace Runic.Agent.Core.Metrics
{
    public interface IStats
    {
        void CountPluginLoaded();
        void CountFlowAdded(string flowName);
        void CountFunctionSuccess(string functionName);
        void CountFunctionFailure(string functionName);
        void SetThreadLevel(string flowName, int threadCount);
    }
}
