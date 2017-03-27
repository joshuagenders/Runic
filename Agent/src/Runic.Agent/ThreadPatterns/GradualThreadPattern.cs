using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.ThreadPatterns
{
    public class GradualThreadPattern : GraphThreadPattern, IThreadPattern
    {
        public int ThreadCount { get; set; }
        public double RampUpSeconds { get; set; }
        public double RampDownSeconds { get; set; }
        public int StepIntervalSeconds { get; set; }

        private DateTime _startTime { get; set; }
        private DateTime _rampupEndTime { get; set; }
        private DateTime _rampdownStartTime { get; set; }
        private DateTime _endTime { get; set; }
        
        private void GenerateTimes()
        {
            _startTime = DateTime.Now;
            _rampupEndTime = _startTime.AddSeconds(RampUpSeconds);
            _endTime = _startTime.AddSeconds(DurationSeconds);
            _rampdownStartTime = _endTime.AddSeconds(-RampDownSeconds);
        }

        private void GeneratePoints()
        {
            Points = new List<Point>();

            //add ramp up points
            var rampUpSeconds = (_rampupEndTime - _startTime).Seconds;
            var rampupIntervalCount = rampUpSeconds / StepIntervalSeconds;
            for (int i = 1; i < rampupIntervalCount; i++)
            {
                Points.Add(new Point()
                {
                    unitsFromStart = i * StepIntervalSeconds,
                    threadLevel = (ThreadCount / rampupIntervalCount) * i
                });
            }
            //add max thread level
            Points.Add(new Point()
            {
                unitsFromStart = rampUpSeconds,
                threadLevel = ThreadCount
            });

            //add ramp down points
            var rampdownStartX = (_rampdownStartTime - _startTime).Seconds;
            var rampdownIntervalCount = RampDownSeconds / StepIntervalSeconds;

            for (int i = 1; i < rampdownIntervalCount; i++)
            {
                Points.Add(new Point()
                {
                    unitsFromStart = rampdownStartX + (i * StepIntervalSeconds),
                    threadLevel = (ThreadCount / rampdownIntervalCount) * i
                });
            }

            //add end point
            Points.Add(new Point()
            {
                unitsFromStart = DurationSeconds,
                threadLevel = 0
            });
        }

        public override async Task Start(CancellationToken ct)
        {
            GenerateTimes();
            GeneratePoints();
            await base.Start(ct);
        }
    }
}
