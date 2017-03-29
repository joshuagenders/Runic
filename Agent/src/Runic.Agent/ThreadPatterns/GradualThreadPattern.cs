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
        public int RampUpSeconds { get; set; }
        public int RampDownSeconds { get; set; }
        public int StepIntervalSeconds { get; set; }

        private void AddStartPoint()
        {
            Points.Add(new Point()
            {
                threadLevel = 0,
                unitsFromStart = 0
            });
        }

        private void AddEndPoint()
        {
            var lastPoint = Points[Points.Count - 1];
            if (lastPoint.threadLevel != 0)
            {
                Points.Add(new Point()
                {
                    unitsFromStart = lastPoint.unitsFromStart,
                    threadLevel = 0
                });
            }
        }

        private void AddRampUpPoints()
        {
            //add ramp up points
            int rampupIntervalCount = RampUpSeconds / StepIntervalSeconds;
            if (rampupIntervalCount <= 0)
                rampupIntervalCount = 1;
            var startX = Points[Points.Count - 1].unitsFromStart + 1;

            var previousThreadCount = 0;
            var nextThreadCount = 0;
            int stepAmount = ThreadCount / rampupIntervalCount;
            if (stepAmount < 1)
                stepAmount = 1;

            for (int i = 1; i <= rampupIntervalCount; i++)
            {    
                nextThreadCount = stepAmount * i;
                if (nextThreadCount != previousThreadCount)
                {
                    Points.Add(new Point()
                    {
                        unitsFromStart = (i * StepIntervalSeconds) + startX,
                        threadLevel = nextThreadCount
                    });
                    previousThreadCount = nextThreadCount;
                }
            }
        }

        private void AddMaxLevelPoint()
        {
            var lastPoint = Points[Points.Count - 1];
            if (lastPoint.threadLevel != ThreadCount)
            {
                //add max thread level
                Points.Add(new Point()
                {
                    unitsFromStart = lastPoint.unitsFromStart + 1,
                    threadLevel = ThreadCount
                });
            }
        }

        private void AddRampdownPoints()
        {
            //add ramp down points
            int rampdownIntervalCount = RampDownSeconds / StepIntervalSeconds;
            if (rampdownIntervalCount <= 0)
                rampdownIntervalCount = 1;

            var lastPoint = Points[Points.Count - 1];
            var startX = DurationSeconds - RampDownSeconds > lastPoint.unitsFromStart 
                ? DurationSeconds - RampDownSeconds 
                : lastPoint.unitsFromStart + 1;

            if (startX <= 1)
                startX = 1;

            var previousThreadCount = lastPoint.threadLevel;
            int stepAmount = ThreadCount / rampdownIntervalCount;
            if (stepAmount < 1)
                stepAmount = 1;

            for (int i = 1; i <= rampdownIntervalCount; i++)
            {
                var nextThreadCount = previousThreadCount - stepAmount;
                if (nextThreadCount != previousThreadCount)
                {
                    Points.Add(new Point()
                    {
                        unitsFromStart = (i * StepIntervalSeconds) + startX,
                        threadLevel = nextThreadCount
                    });
                    previousThreadCount = nextThreadCount;
                }
            }
        }

        private void GeneratePoints()
        {
            Points = new List<Point>();

            AddStartPoint();

            if (RampUpSeconds > 0)
                AddRampUpPoints();

            AddMaxLevelPoint();

            if (RampDownSeconds > 0)
                AddRampdownPoints();

            AddEndPoint();
        }

        public override async Task Start(CancellationToken ct)
        {
            GeneratePoints();
            await base.Start(ct);
        }
    }
}
