namespace Runic.Agent.Framework.Models
{
    public struct Point
    {
        //time value, calculated as ratio of max x
        private int unitsFromStart;
        //thread level
        private int threadLevel;

        public int UnitsFromStart { get => unitsFromStart; set => unitsFromStart = value; }
        public int ThreadLevel { get => threadLevel; set => threadLevel = value; }
    }
}
