using StatsN;

namespace Runic.Agent.Metrics
{
    public static class Stats
    {
        public static IStatsd Statsd { get; set; }

        public static void CountPluginLoaded()
        {
            Statsd?.Count("plugins.pluginLoaded");
        }

        public static void CountFlowAdded(string flowName)
        {
            Statsd?.Count($"flows.{flowName}.AddOrUpdated");
        }

        public static void CountFunctionSuccess(string functionName)
        {
            Statsd?.Count($"functions.{functionName}.actions.execute.success");
        }

        public static void CountFunctionFailure(string functionName)
        {
            Statsd?.Count($"functions.{functionName}.actions.execute.success");
        }

        public static void SetThreadLevel(string flowName, int threadCount)
        {
            Statsd?.Gauge($"flows.{flowName}.threadLevel", threadCount);
        }
    }
}
