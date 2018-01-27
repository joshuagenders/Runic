using Akka.Actor;
using Runic.Agent.Standalone.Messages;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Standalone.Actors
{
    public class ProducerSuperVisor : ReceiveActor
    {
        private readonly IActorRef _consumerSupervisor;
        private List<IActorRef> _producers { get; set; }

        public ProducerSuperVisor(IActorRef consumerSupervisor)
        {
            _producers = new List<IActorRef>();
            _consumerSupervisor = consumerSupervisor;

            Receive<StartProducer>(_ => CreateProducer(_));
            //on terminate?
        }

        private void CreateProducer(StartProducer startProducer)
        {
            var producer = 
                Context.ActorOf(
                    Props.Create<Producer>(_consumerSupervisor, startProducer.TestPlan));
            Context.System
                   .Scheduler
                   .ScheduleTellRepeatedly(
                        TimeSpan.FromSeconds(0),
                        TimeSpan.FromSeconds(5),
                        producer,
                        new Produce(),
                        Self);
            _producers.Add(producer);   
        }
    }
}
