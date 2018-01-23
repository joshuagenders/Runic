using Akka.Actor;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.Standalone.Messages;
using System;
using System.Threading;

namespace Runic.Agent.Standalone.Actors
{
    public class Producer : ReceiveActor
    {
        private DateTime? _lastPollTime { get; set; }
        private readonly IActorRef _consumerSupervisor;
        public Producer(IActorRef consumerSupervisor)
        {
            _consumerSupervisor = consumerSupervisor;
            Receive<ProduceTestPlan>(_ => ProduceTestPlans(_));
        }

        private void ProduceTestPlans(ProduceTestPlan testPlan)
        {
            while (true)
            {
                PopulateWorkQueue(testPlan.TestPlan);
                Thread.Sleep(1000);
            }
        }

        private int GetJourneyCountInPeriod(FrequencyPattern frequencyPattern, DateTime startTime, DateTime endTime)
        {
            //store and reuse calculation total
            var totalSeconds = endTime.Subtract(startTime).TotalSeconds;
            double total = 0;
            for (int i = 0; i < totalSeconds; i++)
            {
                total += new FrequencyService()
                    .GetCurrentFrequencyPerMinute(
                        frequencyPattern, 
                        startTime, 
                        startTime.AddSeconds(i)) / 60;
            }

            return (int)total;
        }

        private void PopulateWorkQueue(TestPlan testPlan)
        {
            var lastTime = _lastPollTime;
            var now = DateTime.Now;
            _lastPollTime = now;

            var count =
                GetJourneyCountInPeriod(testPlan.Frequency, testPlan.StartTime, now)
                    - GetJourneyCountInPeriod(testPlan.Frequency, testPlan.StartTime, lastTime.Value);

            for (int i = 0; i < count; i++)
            {
                //todo
                _consumerSupervisor.Tell(new ExecuteTestPlan() { TestPlan = testPlan });
            }
        }
    }
}
