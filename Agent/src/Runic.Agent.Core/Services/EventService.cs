using System;
using Runic.Agent.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Core.Services
{
    public class EventService : IEventService
    {
        private readonly List<IEventHandler> _eventHandlers;

        public EventService(List<IEventHandler> eventHandlers = null)
        {
            _eventHandlers = eventHandlers;
        }

        public void Debug(string message, Exception ex = null) => _eventHandlers?.ForEach(h => h.Debug(message, ex));
        public void Info(string message, Exception ex = null) => _eventHandlers?.ForEach(h => h.Info(message, ex));
        public void Warning(string message, Exception ex = null) => _eventHandlers?.ForEach(h => h.Warning(message, ex));
        public void Error(string message, Exception ex = null) => _eventHandlers?.ForEach(h => h.Error(message, ex));
        //public void OnTestResult(Result result) => _eventHandlers?.ForEach(h => h.OnTestResult(result));
        public void OnFlowStart(Flow flow) => _eventHandlers?.ForEach(h => h.OnFlowStart(flow));
        public void OnFlowComplete(Flow flow) => _eventHandlers?.ForEach(h => h.OnFlowComplete(flow));
        public void OnThreadChange(Flow flow, int threadLevel) => _eventHandlers?.ForEach(h => h.OnThreadChange(flow, threadLevel));
    }
}
