using Akka.Actor;
using Runic.Agent.Core.Actors;
using Runic.Agent.Core.Messages;
using System;

namespace Runic.Agent.Standalone
{
    public static class Program
    {   
        public static void Main(string[] args)
        {
            try
            {
                var config = new ConfigBuilder(args).Config;
                var testPlans = TestPlanLoader.GetTestPlans(config);

                using (var system = ActorSystem.Create("runic-system"))
                {
                    // Create top level supervisor
                    var app = system.ActorOf(Props.Create<RootSupervisor>(), "root-supervisor");

                    foreach (var plan in testPlans)
                    {
                        app.Tell(new ExecuteTestPlan(plan));
                    }
                    // Exit the system after ENTER is pressed
                    Console.WriteLine("Press Enter to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred: {ex.Message}");
                Console.ReadLine();
            }
        }
    }
}