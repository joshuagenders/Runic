namespace Runic.Agent.Framework.ExternalInterfaces
{
    public interface ITestDataService
    {
        object GetMethodParameterValue(string datasourceId, string datasourceKey);
    }
}
