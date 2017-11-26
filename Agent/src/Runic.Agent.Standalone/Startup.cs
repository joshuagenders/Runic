using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.WorkGenerator;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.IoC;
using System.Collections.Generic;

namespace Runic.Agent.Standalone
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            CoreServiceCollection.ConfigureServices(services);
        }

        public static Dictionary<string,Work> GetTestPlans()
        {
            //todo read from config
            return new Dictionary<string, Work>();
        }
    }
}
