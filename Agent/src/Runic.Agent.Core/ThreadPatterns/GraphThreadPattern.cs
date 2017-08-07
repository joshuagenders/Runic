using Runic.Agent.Core.Services;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class GraphThreadPattern : IThreadPattern
    {
        private readonly IDatetimeService _datetimeService;

        private IList<Action<int>> _callbacks { get; set; }

        public IList<Point> Points { get; set; }
        public int DurationSeconds { get; set; }

        public GraphThreadPattern(IDatetimeService datetimeService)
        {
            _callbacks = new List<Action<int>>();
            _datetimeService = datetimeService;
        }

        public void RegisterThreadChangeHandler(Action<int> callback) => _callbacks.Add(callback);
        public virtual int GetMaxDurationSeconds() => DurationSeconds;
        public virtual int GetMaxThreadCount() => Points.Max(p => p.threadLevel);

        public virtual async Task StartPatternAsync(CancellationToken ctx = default(CancellationToken))
        {
            double maxX = Points.Max(p => p.unitsFromStart);
            for (int index = 0; index < Points.Count; index++)
            {
                if (ctx.IsCancellationRequested)
                    return;
                var currentPoint = Points[index];

                _callbacks.ToList().ForEach(setthread => setthread(currentPoint.threadLevel));

                if (index < Points.Count - 1)
                {
                    var nextPoint = Points[index + 1];
                    var waitTimeSeconds = ((nextPoint.unitsFromStart - currentPoint.unitsFromStart) * (DurationSeconds / maxX));
                    await _datetimeService.WaitUntil((int)waitTimeSeconds * 1000, ctx);
                }
            }
            await Task.CompletedTask;
        }
    }
}
