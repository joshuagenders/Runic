namespace Runic.Agent.Core.Models
{
    public class Step
    {
        public string StepName { get; set; }
        public int RepeatCount { get; set; }
        public DataInput DataInput { get; set; }
        public MethodInformation Function { get; set; }
        public CucmberInformation Cucumber { get; set; }
    }
}
