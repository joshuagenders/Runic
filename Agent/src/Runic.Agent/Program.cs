using Autofac;

namespace Runic.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var startup = new Startup();
            var container = startup.BuildContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var agent = scope.Resolve<IApplication>();
                agent.Run().Wait();
            }
        }
    }
}