namespace Runic.Agent.Core.Models
{
    public class Step
    {
        public Step(
            string stepName, 
            MethodStepInformation function, 
            CucumberStepInformation cucumber)
        {
            StepName = stepName;
            Function = function;
            Cucumber = cucumber;
        }
        
        public string StepName { get; }
        public MethodStepInformation Function { get; set; }
        public CucumberStepInformation Cucumber { get; set; }
    }
}
