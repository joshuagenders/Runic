using Runic.Agent.Core.ExternalInterfaces;
using Runic.Framework.Models;
using System.Threading;

namespace Runic.Agent.Worker
{
    public class AgentObserver : IAgentObserver
    {
        private readonly SemaphoreSlim _semaphore;
        private FlowInformation _flowInformation { get; set; }
        private PatternInformation _patternInformation { get; set; }

        public FlowInformation FlowInformation
        {
            get
            {
                try
                {
                    _semaphore.Wait();
                    return _flowInformation;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            set
            {
                try
                {
                    _semaphore.Wait();
                    _flowInformation = value;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public PatternInformation PatternInformation
        {
            get
            {
                try
                {
                    _semaphore.Wait();
                    return _patternInformation;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            set
            {
                try
                {
                    _semaphore.Wait();
                    _patternInformation = value;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public AgentObserver()
        {
            _semaphore = new SemaphoreSlim(1);
        }

        public void Update(FlowInformation flowInformation)
        {
            FlowInformation = flowInformation;
        }

        public void Update(PatternInformation patternInformation)
        {
            PatternInformation = patternInformation;
        }
    }
}
