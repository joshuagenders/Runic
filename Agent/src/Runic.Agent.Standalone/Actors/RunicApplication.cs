﻿using Akka.Actor;
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
        private ICancelable _producerCancellation { get; set; }

        public RunicApplication()
        {
            _consumerSupervisor = Context.ActorOf<ConsumerSupervisor>("consumer-supervisor");
            _producerSupervisor = Context.ActorOf<ProducerSuperVisor>("producer-supervisor");

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