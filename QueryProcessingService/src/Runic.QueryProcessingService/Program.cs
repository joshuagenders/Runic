﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Runic.Core.Models;

namespace Runic.QueryProcessingService
{
    public class Program
    {
        public static string AgentId { get; set; }
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            AgentId = Guid.NewGuid().ToString("n");
            LoadConfig();
            var program = new Program();
            var token = new CancellationToken();
            Task.Run(() => { program.Run(token); }, token).ContinueWith(_ =>
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
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void Run(CancellationToken token)
        {
            using (var bus =
                RabbitHutch.CreateBus("host=localhost;virtualHost=myVirtualHost;username=mike;password=topsecret"))
            {
                AssignResponders(bus);
                while (!token.IsCancellationRequested)
                    Thread.Sleep(500);
            }
        }

        public void AssignResponders(IBus bus)
        {
            bus.Respond<RuneQueryRequest, RuneQueryResponse>(request =>
                new QueryProcessingService().ProcessRuneQuery(request));
        }
    }
}