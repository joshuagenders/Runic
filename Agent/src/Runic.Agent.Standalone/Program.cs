using Akka.Actor;
using Runic.Agent.Standalone.Actors;
using Runic.Agent.Standalone.Messages;
using System;

namespace Runic.Agent.Standalone
{
    public static class Program
    {   
        public static void Main(string[] args)
        {
            var config = new ConfigBuilder(args).Config;
            var testPlans = TestPlanLoader.GetTestPlans(config);

            using (var system = ActorSystem.Create("runic-system"))
            {
                // Create top level supervisor
                var app = system.ActorOf(Props.Create<RunicApplication>(), "application");

                foreach (var plan in testPlans)
                {
                    app.Tell(new ExecuteTestPlan() { TestPlan = plan });
                }
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }
        }
    }
}