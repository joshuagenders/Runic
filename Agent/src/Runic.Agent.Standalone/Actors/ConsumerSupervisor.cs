using Akka.Actor;
using Runic.Agent.Standalone.Messages;

namespace Runic.Agent.Standalone.Actors
{
    public class ConsumerSupervisor : ReceiveActor
    {
        public ConsumerSupervisor()
        {
            Receive<ExecuteTestPlan>(_ => CreateConsumer(_));
            //todo error handling?
          
        }

        private void CreateConsumer(ExecuteTestPlan testPlan)
        {
            var consumer = Context.ActorOf<Consumer>();
            consumer.Tell(testPlan.TestPlan);
        }
    }
}
