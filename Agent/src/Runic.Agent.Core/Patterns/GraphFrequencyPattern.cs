using Runic.Agent.Services;
using Runic.Agent.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Core.Patterns
{
    public class GraphFrequencyPattern : IFrequencyPattern
    {    
        public IList<Point> Points { get; set; }
        public int DurationSeconds { get; set; }
        public virtual string PatternType => "graph";
        public double MaxJourneysPerMinute => Points.Max(j => j.FrequencyPerMinute);
        
        public double GetCurrentFrequencyPerMinute(DateTime startTime, DateTime now)
        {
            if (now > startTime.AddSeconds(DurationSeconds)
             || now < startTime)
            {
                return 0;
            }
            
            double maxX = Points.Max(p => p.UnitsFromStart);
            double secondsPerPoint = (DurationSeconds / maxX);
            var timeEllapsedSeconds = now.Subtract(startTime).Seconds;
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
            return point.FrequencyPerMinute;
        }
    }
}
