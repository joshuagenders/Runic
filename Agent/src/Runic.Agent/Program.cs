using System.Threading;

namespace Runic.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var agent = new Agent();
            var startup = new Startup();
            var cts = new CancellationTokenSource();
            agent.Start(args, startup, cts.Token).Wait();
        }
    }
}