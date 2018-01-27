using Akka.Actor;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.WorkGenerator;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Standalone
{
    public class Consumer : ReceiveActor
    {
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
            //
        }

        private void ExecuteTestPlan(TestPlan testPlan)
        {
            var person = new Person(_assemblyManager);
            person.PerformJourneyAsync(testPlan.Journey).PipeTo(Self);
        }
    }
}