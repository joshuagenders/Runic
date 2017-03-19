using NLog;
using Runic.Framework.Configuration;
using StatsN;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Runic.Framework.Orchestration
{
    public static class Extensions
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static readonly Statsd Client = new Statsd(RunicConfiguration.GetStatsdConfiguration);

        public static async Task<ActionResult> TimedExecute (this Action action, string actionName)
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
                _logger.Error(e);
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

        public static async Task<FuncResult<T>> TimedExecute <T>(this Func<T> func, string actionName)
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
                _logger.Error(e);
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
