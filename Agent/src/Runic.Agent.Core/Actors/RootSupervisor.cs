using Akka.Actor;
using Akka.Event;
using Runic.Agent.Core.Messages;

namespace Runic.Agent.Core.Actors
{
    public class RootSupervisor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Runic Application started");
        protected override void PostStop() => Log.Info("Runic Application stopped");

        private IActorRef _producerSupervisor { get; set; }
        private IActorRef _consumerSupervisor { get; set; }
        private IActorRef _resultProcessor { get; set; }

        private ICancelable _producerCancellation { get; set; }

        public RootSupervisor()
        {
            _consumerSupervisor = Context.ActorOf<ConsumerSupervisor>("consumer-supervisor");
            _producerSupervisor = Context.ActorOf<ProducerSuperVisor>("producer-supervisor");
            _resultProcessor = Context.ActorOf<ResultProcessor>("result-processor");

            //shutdown on completion - ask producer supervisor if there are active producers
            _producerCancellation = Context.System
                                           .Scheduler
                                           .ScheduleTellRepeatedlyCancelable(
                                                initialMillisecondsDelay: 10000, 
                                                millisecondsInterval: 15000, 
                                                receiver: _producerSupervisor, 
                                                message: new IfNoProducers(), 
                                                sender: Self);
            Receive<ExecuteTestPlan>(_ => ExecutePlan(_));
            Receive<NoProducers>(_ => Shutdown());
        }

        private void Shutdown()
        {
            Context.Stop(Self);
        }

        private void ExecutePlan(ExecuteTestPlan testPlan)
        {
            _producerSupervisor.Tell(new StartProducer(testPlan.TestPlan));
        }
    }
}