using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Runic.Configuration;
using StatsN;

namespace Runic.Orchestration
{
    public class TimedAction
    {
        /// <summary>
        ///     The _client.
        /// </summary>
        private static readonly Statsd Client = new Statsd(AppConfiguration.GetStatsdConfiguration);

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimedAction" /> class.
        /// </summary>
        /// <param name="actionName">
        ///     The action name.
        /// </param>
        /// <param name="action">
        ///     The action.
        /// </param>
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

        /// <summary>
        ///     Gets the Action name.
        /// </summary>
        private string ActionName { get; }

        /// <summary>
        ///     Gets the action.
        /// </summary>
        private Delegate Action { get; }

        /// <summary>
        ///     Gets the stopwatch.
        /// </summary>
        private Stopwatch Stopwatch { get; }

        public async Task<ActionResult> Execute()
        {
            object result;
            try
            {
                Stopwatch.Start();
                result = await Task.Run(() => Action.DynamicInvoke());
            }
            catch (Exception e)
            {
                Client.Count($"{ActionName}.Exception.{e.GetType().FullName}");
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

            return new ActionResult
            {
                ElapsedMilliseconds = Stopwatch.ElapsedMilliseconds,
                ExecutionResult = result
            };
        }
    }
}