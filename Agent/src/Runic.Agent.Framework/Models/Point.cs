namespace Runic.Agent.Framework.Models
{
    public struct Point
    {
        //time value, calculated as ratio of max x
        private int unitsFromStart;
        private double frequencyPerMinute;

        public int UnitsFromStart { get => unitsFromStart; set => unitsFromStart = value; }
        public double FrequencyPerMinute { get => frequencyPerMinute; set => frequencyPerMinute = value; }
    }
}
