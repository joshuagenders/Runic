using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.ThreadPatterns
{
    public class GraphThreadPattern : IThreadPattern
    {
        private List<Action<int>> _callbacks { get; set; }

        public List<Point> Points { get; set; }
        public int DurationSeconds { get; set; }

        public GraphThreadPattern()
        {
            _callbacks = new List<Action<int>>();
        }

        public void RegisterThreadChangeHandler(Action<int> callback)
        {
            _callbacks.Add(callback);
        }

        public virtual async Task Start(CancellationToken ct)
        {
            var maxX = Points.Max(p => p.unitsFromStart);
            for (int index = 0; index <= Points.Count; index++)
            {
                if (ct.IsCancellationRequested)
                    return;
                //overwrites last point with 0 threads for safety..think about this
                if (index == Points.Count - 1)
                {
                    _callbacks.ForEach(setthread => setthread(0));
                    break;
                }
                var currentPoint = Points[index];
                _callbacks.ForEach(setthread => setthread((int)currentPoint.threadLevel));

                var nextPoint = Points[index + 1];
                var waitTimeSeconds = ((nextPoint.unitsFromStart - currentPoint.unitsFromStart) / maxX) * DurationSeconds;
                var cancelled = ct.WaitHandle.WaitOne(TimeSpan.FromSeconds(waitTimeSeconds));
            }
        }
    }
}
