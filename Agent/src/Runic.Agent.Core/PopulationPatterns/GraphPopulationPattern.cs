using Runic.Agent.TestHarness.Services;
using Runic.Agent.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class GraphPopulationPattern : IPopulationPattern
    {
        private readonly IDatetimeService _datetimeService;
        
        public IList<Point> Points { get; set; }
        public int DurationSeconds { get; set; }

        public GraphPopulationPattern(IDatetimeService datetimeService)
        {
            _datetimeService = datetimeService;
        }

        public virtual int GetMaxDurationSeconds() => DurationSeconds;
        public virtual int GetMaxPersonCount() => Points.Max(p => p.PopulationSize);

        public List<Tuple<DateTime, int>> GetThreadChangeEvents(DateTime startTime)
        {
            double maxX = Points.Max(p => p.UnitsFromStart);
            double secondsPerPoint = (DurationSeconds / maxX);
            var threadEvents = Points.Select(p => Tuple.Create(startTime.AddSeconds(secondsPerPoint * p.UnitsFromStart), p.PopulationSize)).ToList();
            threadEvents.Add(Tuple.Create(startTime.AddSeconds(DurationSeconds), 0));
            return threadEvents;
        }

        public virtual string GetPatternType() => "graph";

        public int GetCurrentActivePopulationCount(DateTime startTime)
        {
            if (_datetimeService.Now > startTime.AddSeconds(DurationSeconds))
            {
                return 0;
            }
            if (_datetimeService.Now < startTime)
            {
                return 0;
            }
            
            double maxX = Points.Max(p => p.UnitsFromStart);
            double secondsPerPoint = (DurationSeconds / maxX);
            var timeEllapsedSeconds = _datetimeService.Now.Subtract(startTime).Seconds;
            var currentPosition = timeEllapsedSeconds / secondsPerPoint;

            Point point = Points[0];
            for (var index = 1; index < Points.Count; index++)
            {
                if (point.UnitsFromStart > currentPosition)
                {
                    break;
                }
                if (currentPosition < Points[index].UnitsFromStart)
                    break;
                point = Points[index];
            }
            return point.PopulationSize;
        }
    }
}
