using Akka.Actor;
using Runic.Agent.Standalone.Messages;
using System.Collections.Generic;

namespace Runic.Agent.Standalone.Actors
{
    public class ConsumerSupervisor : ReceiveActor
    {
        private readonly List<IActorRef> _consumers;
        
        //gracefully handle terminate of producer (finished/terminated)

        public ConsumerSupervisor()
        {
            _consumers = new List<IActorRef>();
            Receive<ExecuteTestPlan>(_ => AssignTestToConsumer(_));
            Receive<Terminated>(_ => RemoveConsumer(_));
        }

        private void AssignTestToConsumer(ExecuteTestPlan testPlan)
        {
            CreateConsumer().Tell(testPlan.TestPlan); ;
        }
        
        private void RemoveConsumer(Terminated terminated)
        {
            _consumers.RemoveAll(r => r == terminated.ActorRef);
        }

        private IActorRef CreateConsumer()
        {
            //todo use persistent actors and control consumer counts to aid restart times?
            var consumer = Context.ActorOf<Consumer>();
            Context.Watch(consumer);
            _consumers.Add(consumer);
            return consumer;
        }
    }
}
