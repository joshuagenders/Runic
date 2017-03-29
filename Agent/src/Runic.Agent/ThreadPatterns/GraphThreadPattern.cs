﻿using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.ThreadPatterns
{
    public class GraphThreadPattern : IThreadPattern
    {
        private IList<Action<int>> _callbacks { get; set; }

        public IList<Point> Points { get; set; }
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
            if (Points[Points.Count - 1].threadLevel != 0)
            {
                Points[Points.Count - 1] = new Point()
                {
                    threadLevel = 0,
                    unitsFromStart = DurationSeconds
                };
            }

            var maxX = Points.Max(p => p.unitsFromStart);
            for (int index = 0; index < Points.Count; index++)
            {
                if (ct.IsCancellationRequested)
                    return;
                var currentPoint = Points[index];

                _callbacks.ToList().ForEach(setthread => setthread((int)currentPoint.threadLevel));

                if (index < Points.Count - 1)
                {
                    var nextPoint = Points[index + 1];
                    var waitTimeSeconds = ((nextPoint.unitsFromStart - currentPoint.unitsFromStart) / maxX) * DurationSeconds;
                    var cancelled = ct.WaitHandle.WaitOne(TimeSpan.FromSeconds(waitTimeSeconds));
                }
            }
        }
    }
}