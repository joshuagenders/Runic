using Runic.Agent.Core.Exceptions;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Core.Services
{
    public class DistributionStepController : IStepController
    {
        private List<Step> _steps { get; set; }
        private ConcurrentQueue<Step> _stepQueue { get; set; }

        public int QueueSize => _stepQueue.Count;

        public DistributionStepController(List<Step> steps)
        {
            _steps = steps;
            _stepQueue = new ConcurrentQueue<Step>();
        }

        public Step GetNextStep(Result result)
        {
            if (_stepQueue.IsEmpty)
                PopulateStepQueue();
            var retryCount = 10;
            Step step = null;
            while (step == null && retryCount > 0)
            {
                retryCount--;
                if (_stepQueue.TryDequeue(out step))
                    return step;
            }
            throw new StepDequeueFailedException();
        }

        private void PopulateStepQueue()
        {
            var gcd = GCD(_steps.Select(s => Convert.ToInt32(s.Distribution.DistributionAllocation))
                                .ToArray());

            var distributions = new Dictionary<Step, int>();
            var totalMessageCount = 0;
            foreach (var step in _steps)
            {
                var reducedValue = (int)step.Distribution.DistributionAllocation / gcd;
                distributions[step] = reducedValue;
                totalMessageCount += reducedValue;
            }

            var messageCount = 0;
            while (messageCount < totalMessageCount)
            {
                var orderedDistributions = distributions.OrderByDescending(d => d.Value).ToList();
                foreach (var step in orderedDistributions)
                {
                    if (distributions[step.Key] > 0)
                    {
                        distributions[step.Key]--;
                        messageCount++;
                        _stepQueue.Enqueue(step.Key);
                    }
                }
            }
        }

        private static int GCD(int[] values)
        {
            return values.Aggregate(FindGCD);
        }

        private static int FindGCD(int value1, int value2)
        {
            while (value1 != 0 && value2 != 0)
            {
                if (value1 > value2)
                {
                    value1 %= value2;
                }
                else
                {
                    value2 %= value1;
                }
            }
            return Math.Max(value1, value2);
        }

    }
}
