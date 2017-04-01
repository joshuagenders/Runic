using System;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var agent = new Agent();
            var startup = new Startup();
            Task.Run(async () =>
            {
                await agent.Start(args, startup).ContinueWith((result) =>
                {
                    if (result.Exception != null)
                    {
                        Console.WriteLine("An error occured.");
                    }
                    Console.WriteLine("Exiting.");
                });
            });
        }
    }
}