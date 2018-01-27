using Akka.Actor;
using Runic.Agent.Standalone.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Standalone.Actors
{
    public class ProducerSuperVisor : ReceiveActor
    {
        private Dictionary<IActorRef, ICancelable> _producerScheduleCancellers { get; set; }

        public ProducerSuperVisor()
        {
            _producerScheduleCancellers = new Dictionary<IActorRef, ICancelable>();
            Receive<StartProducer>(_ => CreateProducer(_));
            Receive<Terminated>(_ => HandleTerminate(_));
            Receive<IfNoProducers>(_ => TellIfNoProducers());
        }

        private void TellIfNoProducers()
        {
            if (!Context.GetChildren().Any())
                Sender.Tell(new NoProducers());
        }

        private void HandleTerminate(Terminated terminated)
        {
            ICancelable cancelable;
            _producerScheduleCancellers.TryGetValue(terminated.ActorRef, out cancelable);

            cancelable?.CancelIfNotNull();
            Context.Stop(terminated.ActorRef);
            _producerScheduleCancellers.Remove(terminated.ActorRef);
        }

        private void CreateProducer(StartProducer startProducer)
        {
            var producer = 
                Context.ActorOf(
                    Props.Create<Producer>(startProducer.TestPlan));

            var cancellation = new Cancelable(Context.System.Scheduler);
            Context.System
                   .Scheduler
                   .ScheduleTellRepeatedly(
                        TimeSpan.FromSeconds(0),
                        TimeSpan.FromSeconds(5),
                        producer,
                        new Produce(),
                        Self,
                        cancellation);

            _producerScheduleCancellers[producer] = cancellation;   
        }
    }
}
