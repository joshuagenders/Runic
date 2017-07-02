using Microsoft.Extensions.Logging;
using StatsN;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Runic.Framework.Extensions
{
    public static class ActionExtensions
    {
        private static readonly ILogger _logger = new LoggerFactory().CreateLogger(nameof(ActionExtensions));

        public static Statsd Client { get; set; }

        public static async Task<ActionResult> TimedExecute(this Action action, string actionName)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                await Task.Run(() => action());
            }
            catch (Exception e)
            {
                Client.Count($"{actionName}.Exception.{e.GetType().FullName}");
                _logger.LogError("An error occured in timed execute", e);
                return new ActionResult
                {
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    ExecutionException = e
                };
            }
            finally
            {
                stopwatch.Stop();
            }

            Client.Count($"{actionName}.Success");
            Client.Timing(actionName, stopwatch.ElapsedMilliseconds);

            return new ActionResult
            {
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }

        public static async Task<FuncResult<T>> TimedExecute<T>(this Func<T> func, string actionName)
        {
            T result = default(T);
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                result = await Task.Run(() => func());
            }
            catch (Exception e)
            {
                Client.Count($"{actionName}.Exception.{e.GetType().FullName}");
                _logger.LogError("An error occured in timed execute", e);
                return new FuncResult<T>
                {
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    ExecutionException = e
                };
            }
            finally
            {
                stopwatch.Stop();
            }

            Client.Count($"{actionName}.Success");
            Client.Timing(actionName, stopwatch.ElapsedMilliseconds);

            return new FuncResult<T>
            {
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                ExecutionResult = result
            };
        }
    }
}