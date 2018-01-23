namespace Runic.Agent.Core.Models
{
    public class Point
    {
        public Point(int unitsFromStart, double frequencyPerMinute)
        {
            UnitsFromStart = unitsFromStart;
            FrequencyPerMinute = frequencyPerMinute;
        }

        public int UnitsFromStart { get; }
        public double FrequencyPerMinute { get; }
    }
}
