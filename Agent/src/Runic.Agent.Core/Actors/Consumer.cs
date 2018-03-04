﻿using Akka.Actor;
using Akka.Event;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Models;

namespace Runic.Agent.Core.Actors
{
    public class Consumer : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        private readonly AssemblyManager _assemblyManager;

        protected override void PreStart() => Log.Info("Consumer started");
        protected override void PostStop() => Log.Info("Consumer stopped");

        public Consumer()
        {
            _assemblyManager = new AssemblyManager();
            Receive<TestPlan>(_ => ExecuteTestPlan(_));
        }

        private void ExecuteTestPlan(TestPlan testPlan)
        {
            Log.Info("Performing journey");
            var person = new Person(_assemblyManager);
            person.PerformJourneyAsync(testPlan.Journey).PipeTo(Context.ActorSelection("/user/root-supervisor/result-processor"));
        }
    }
}