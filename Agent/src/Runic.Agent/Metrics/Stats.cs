using StatsN;

namespace Runic.Agent.Metrics
{
    public class Stats : IStats
    {
        private IStatsd _statsd { get; set; }

        public Stats(IStatsd statsd)
        {
            _statsd = statsd;
        }

        public void CountPluginLoaded()
        {
            _statsd?.Count("plugins.pluginLoaded");
        }

        public void CountFlowAdded(string flowName)
        {
            _statsd?.Count($"flows.{flowName}.AddOrUpdated");
        }

        public void CountFunctionSuccess(string functionName)
        {
            _statsd?.Count($"functions.{functionName}.actions.execute.success");
        }

        public void CountFunctionFailure(string functionName)
        {
            _statsd?.Count($"functions.{functionName}.actions.execute.success");
        }

        public void SetThreadLevel(string flowName, int threadCount)
        {
            _statsd?.Gauge($"flows.{flowName}.threadLevel", threadCount);
        }
    }
}
