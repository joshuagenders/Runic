namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface ITestDataService
    {
        object GetMethodParameterValue(string datasourceId, string datasourceKey);
    }
}
