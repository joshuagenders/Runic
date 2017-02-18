using EasyNetQ;
using Newtonsoft.Json;
using Runic.Core.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Runic.RuneStorageService
{
    public class Program
    {
        public static string AgentId { get; set; }
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            AgentId = Guid.NewGuid().ToString("n");
            LoadConfig();
            Program program = new Program();
            CancellationToken token = new CancellationToken();
            Task.Run(() =>
            {
                program.Run(token);
            }).ContinueWith((_) =>
            {
                if (_.Exception != null)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(_.Exception));
                    Console.WriteLine("Process errored. Exiting.");
                }
            }, token);
        }

        public static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void Run(CancellationToken token)
        {
            using (var bus =
                RabbitHutch.CreateBus("host=localhost;virtualHost=myVirtualHost;username=mike;password=topsecret"))
            {
                AssignSubscribers(bus);
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                }
            }
        }

        public void AssignSubscribers(IBus bus)
        {
            bus.Subscribe<RuneStorageRequest<Rune>>(AgentId, msg => new RuneStorageService().StoreRunes(msg.Runes));
        }
    }
}

