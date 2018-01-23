using Akka.Actor;
using Akka.Event;
using Runic.Agent.Standalone.Messages;
namespace Runic.Agent.Standalone.Actors
{
    public class RunicApplication : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Runic Application started");
        protected override void PostStop() => Log.Info("Runic Application stopped");
        private IActorRef _producerSupervisor { get; set; }
        private IActorRef _consumerSupervisor { get; set; }

        public RunicApplication()
        {
            _consumerSupervisor = 
                Context.ActorOf(
                    Props.Create(() => new ConsumerSupervisor()), "consumer-supervisor");
            _producerSupervisor = 
                Context.ActorOf(Props.Create(() => new ProducerSuperVisor(_consumerSupervisor)), 
                    "producer-supervisor");

            Receive<ExecuteTestPlan>(_ => ExecutePlan(_));
        }

        private void ExecutePlan(ExecuteTestPlan testPlan)
        {
            _producerSupervisor.Tell(new ProduceTestPlan() { TestPlan = testPlan.TestPlan });
        }
    }
}