﻿using Autofac;
using System;

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
                agent.Run()
                     .ContinueWith((result) =>
                     {
                         if (result.Exception != null)
                         {
                             Console.WriteLine("An exception occured.");
                             Console.WriteLine(result.Exception.Message);
                         }
                         Console.WriteLine("Exiting application...");
                     });
            }
        }
    }
}