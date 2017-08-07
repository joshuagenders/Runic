using System;

namespace Runic.Framework.Clients
{
    public interface IStatsClient
    {
        void CountPluginLoaded();
        void CountFlowAdded(string flowName);
        void CountFunctionSuccess(string functionName);
        void CountFunctionFailure(string functionName);
        void SetThreadLevel(string flowName, int threadCount);
        void CountHttpRequestSuccess(string functionName, string responseCode);
        void CountHttpRequestFailure(string functionName, string responseCode);
        void Time(string functionName, string actionName, Action actionToTime);
        void Time(string functionName, string actionName, int millisecondsEllapsed);
    }
}
