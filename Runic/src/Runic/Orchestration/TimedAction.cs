using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using Runic.Configuration;
using StatsN;

namespace Runic.Orchestration
{
    public class TimedAction
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static readonly Statsd Client = new Statsd(AppConfiguration.GetStatsdConfiguration);

        public TimedAction(string actionName, Action action)
        {
            Action = action;
            ActionName = actionName;
            Stopwatch = new Stopwatch();
        }

        public TimedAction(string actionName, Func<object> action)
        {
            Action = action;
            ActionName = actionName;
            Stopwatch = new Stopwatch();
        }

        private string ActionName { get; }
        private Delegate Action { get; }
        private Stopwatch Stopwatch { get; }

        public async Task<ActionResult> Execute()
        {
            _logger.Info("Executing action");
            object result;
            try
            {
                Stopwatch.Start();
                result = await Task.Run(() => Action.DynamicInvoke());
            }
            catch (Exception e)
            {
                Client.Count($"{ActionName}.Exception.{e.GetType().FullName}");
                _logger.Error(e);
                return new ActionResult
                {
                    ElapsedMilliseconds = Stopwatch.ElapsedMilliseconds,
                    ExecutionResult = e
                };
            }
            finally
            {
                Stopwatch.Stop();
            }

            Client.Count($"{ActionName}.Success");
            Client.Timing(ActionName, Stopwatch.ElapsedMilliseconds);
            _logger.Info("Action complete");

            return new ActionResult
            {
                ElapsedMilliseconds = Stopwatch.ElapsedMilliseconds,
                ExecutionResult = result
            };
        }
    }
}