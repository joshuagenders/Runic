using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Runic.Agent.Core.Messages;

namespace Runic.Agent.Core.Actors
{
    public class ConsumerSupervisor : ReceiveActor
    {
        private readonly IActorRef _router;
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("Consumer supervisor started");
        protected override void PostStop() => Log.Info("Consumer supervisor stopped");
        
        public ConsumerSupervisor()
        {
            //todo configure pool size
            _router = 
                Context.System.ActorOf(
                    Props.Create<Consumer>()
                         .WithRouter(new RoundRobinPool(5, new DefaultResizer(1, 200))), 
                    "consumer-pool");

            Receive<ExecuteTestPlan>(_ => AssignTestToConsumer(_));
            //todo gracefully handle terminate of producer (finished/terminated)
        }

        private void AssignTestToConsumer(ExecuteTestPlan testPlan)
        {
            //create router here max size
            _router.Tell(testPlan.TestPlan);
        }
    }
}
