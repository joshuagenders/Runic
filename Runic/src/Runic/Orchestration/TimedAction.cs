using Runic.Configuration;
using StatsN;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Runic.Orchestration
{
    public class TimedAction
    {
        private string _actionName { get; set; }
        private Delegate _action { get; set; }
        private Stopwatch _stopwatch { get; set; }
        private static Statsd _client = new Statsd(AppConfiguration.GetStatsdConfiguration);

        public TimedAction(string actionName, Action action)
        {
            _action = action;
            _actionName = actionName;
            _stopwatch = new Stopwatch();
        }

        public TimedAction(string actionName, Func<object> action)
        {
            _action = action;
            _actionName = actionName;
            _stopwatch = new Stopwatch();
        }

        public async Task<ActionResult> Execute()
        {
            object result;
            try
            {
                _stopwatch.Start();
                result = await Task.Run(() => _action.DynamicInvoke());
            }
            catch (Exception e)
            {
                _client.Count($"{_actionName}.Exception.{e.GetType().FullName}");
                return new ActionResult()
                {
                    ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds,
                    ExecutionResult = e
                };
            }
            finally
            {
                _stopwatch.Stop();
            }
            _client.Count($"{_actionName}.Success");
            _client.Timing(_actionName, _stopwatch.ElapsedMilliseconds);

            return new ActionResult()
            {
                ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds,
                ExecutionResult = result
            };
        }
    }
}
