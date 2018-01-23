using Akka.Actor;
using Runic.Agent.Standalone.Messages;
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

            Receive<ProduceTestPlan>(_ => CreateProducer(_));
        }

        private void CreateProducer(ProduceTestPlan testPlan)
        {
            var producer = Context.ActorOf(Props.Create<Producer>(_consumerSupervisor));
            producer.Tell(testPlan);
            _producers.Add(producer);   
        }
    }
}
