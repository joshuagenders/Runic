namespace Runic.Agent.Framework.Models
{
    public class Step
    {
        public string StepName { get; set; }
        public int RepeatCount { get; set; }
        public DataInput DataInput { get; set; }
        public FunctionInformation Function { get; set; }
        public CucmberInformation Cucumber { get; set; }
    }
}
