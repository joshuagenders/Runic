using Akka.Actor;
using Akka.Event;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Models;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Standalone
{
    public class Consumer : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Console.WriteLine("Test plan started");
        protected override void PostStop() => Console.WriteLine("Test plan stopped");
        private readonly IAssemblyManager _assemblyManager;
        
        public Consumer()
        {
            //todo get config path
            _assemblyManager = new AssemblyManager("");
            Receive<TestPlan>(_ => ExecuteTestPlan(_));
            Receive<List<Result>>(_ => HandleTestResults(_));
        }
        
        private void HandleTestResults(List<Result> results)
        {
            foreach (var result in results)
            {
                if (result.Success)
                {
                    Log.Info("Journey success");
                }
                else
                {
                    Log.Error($"Journey Failure: {result.Exception.Message}");
                }
            }
        }

        private void ExecuteTestPlan(TestPlan testPlan)
        {
            Log.Info("Performing journey");
            var person = new Person(_assemblyManager);
            person.PerformJourneyAsync(testPlan.Journey).PipeTo(Self);
        }
    }
}