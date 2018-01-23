namespace Runic.Agent.Core.Models
{
    public class CucumberStepInformation
    {
        public CucumberStepInformation(string document, string assemblyName)
        {
            Document = document;
            AssemblyName = assemblyName;
        }

        public string Document { get; }
        public string AssemblyName { get; }
    }
}
