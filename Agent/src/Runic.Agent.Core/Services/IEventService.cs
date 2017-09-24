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
        void OnFlowStart(Framework.Models.Journey flow);
        void OnFlowComplete(Framework.Models.Journey flow);
        void OnThreadChange(Framework.Models.Journey flow, int threadLevel);
    }
}
