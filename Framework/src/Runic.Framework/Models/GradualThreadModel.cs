namespace Runic.Framework.Models
{
    public class GradualThreadModel : GraphThreadModel
    {
        public int ThreadCount { get; set; }
        public int RampUpSeconds { get; set; }
        public int RampDownSeconds { get; set; }
        public int StepIntervalSeconds { get; set; }
    }
}
