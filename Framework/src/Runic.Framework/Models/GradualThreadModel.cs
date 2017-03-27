namespace Runic.Framework.Models
{
    public class GradualThreadModel : GraphThreadModel
    {
        public int ThreadCount { get; set; }
        public double RampUpSeconds { get; set; }
        public double RampDownSeconds { get; set; }
        public int StepIntervalSeconds { get; set; }
    }
}
