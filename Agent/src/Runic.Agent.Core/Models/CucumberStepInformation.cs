namespace Runic.Agent.Core.Models
{
    public class CucumberStepInformation
    {
        public CucumberStepInformation(string document, string assemblyPath)
        {
            Document = document;
            AssemblyPath = assemblyPath;
        }

        public string Document { get; }
        public string AssemblyPath { get; }
    }
}
