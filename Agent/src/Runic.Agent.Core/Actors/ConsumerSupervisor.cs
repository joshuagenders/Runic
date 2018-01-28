using Akka.Actor;
using Akka.Event;
using Runic.Agent.Core.Messages;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Core.Actors
{
    public class ConsumerSupervisor : ReceiveActor
    {
        private readonly List<IActorRef> _consumers;

        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Consumer supervisor started");
        protected override void PostStop() => Log.Info("Consumer supervisor stopped");


        //gracefully handle terminate of producer (finished/terminated)

        public ConsumerSupervisor()
        {
            _consumers = new List<IActorRef>();
            Receive<ExecuteTestPlan>(_ => AssignTestToConsumer(_));
            Receive<Terminated>(_ => RemoveConsumer(_));
        }

        private void AssignTestToConsumer(ExecuteTestPlan testPlan)
        {
            CreateConsumer().Tell(testPlan.TestPlan);
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
