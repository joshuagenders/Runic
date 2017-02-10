using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Runic.Orchestration
{
    public class TimedAction
    {
        private Delegate _action { get; set; }
        private Stopwatch _stopwatch { get; set; }

        public TimedAction(string actionName, Action action)
        {
            _action = action;
            _stopwatch = new Stopwatch();
        }

        public TimedAction(string actionName, Func<object> action)
        {
            _action = action;
            _stopwatch = new Stopwatch();
        }

        public async Task<ActionResult> Execute()
        {
            _stopwatch.Start();
            var result = await Task.Run(() => _action.DynamicInvoke());
            _stopwatch.Stop();
            return new ActionResult()
            {
                ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds,
                ExecutionResult = result
            };
        }
    }
}
