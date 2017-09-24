namespace Runic.Agent.Framework.Models
{
    public struct Point
    {
        //time value, calculated as ratio of max x
        private int unitsFromStart;
        //thread level
        private int populationsSize;

        public int UnitsFromStart { get => unitsFromStart; set => unitsFromStart = value; }
        public int PopulationSize { get => populationsSize; set => populationsSize = value; }
    }
}
