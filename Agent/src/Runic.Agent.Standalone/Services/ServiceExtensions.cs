using Runic.Framework.Models;
using System.Collections.Generic;

namespace Runic.Agent.Standalone.Services
{
    public static class ServiceExtensions
    {
        public static List<Point> ParsePoints(this string[] value)
        {
            var points = new List<Point>();
            foreach (var point in value)
            {
                var split = point.Split('.');
                points.Add(new Point()
                {
                    unitsFromStart = int.Parse(split[0]),
                    threadLevel = int.Parse(split[1])
                });
            }
            return points;
        } 
    }
}
