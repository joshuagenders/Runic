namespace Runic.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var agent = new Agent();
            agent.Start(args).Wait();
        }
    }
}