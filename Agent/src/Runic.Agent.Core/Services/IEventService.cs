using Runic.Agent.Framework.Models;
using System;

namespace Runic.Agent.Core.Services
{
    public interface IEventService
    {
        void Info(string message, Exception ex = null);
        void Debug(string message, Exception ex = null);
        void Warning(string message, Exception ex = null);
        void Error(string message, Exception ex = null);

        //void OnTestResult(Result result);
        void OnFlowStart(Flow flow);
        void OnFlowComplete(Flow flow);
        void OnThreadChange(Flow flow, int threadLevel);
    }
}
