using Runic.Agent.Core.ExternalInterfaces;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.Core.Services
{
    public class DataService : IDataService
    {
        private readonly ITestDataService _testDataService;

        public DataService(ITestDataService testDataService)
        {
            _testDataService = testDataService;
        }
        
        public object[] GetParams(string[] positionalParameters, MethodInfo methodInfo)
        {
            if (positionalParameters == null || !positionalParameters.Any())
            {
                return new object[] { };
            }
            var testDataMappedValues =
                positionalParameters
                    .Select(p => p.StartsWith("=>") && p.Contains(".") 
                                 ? _testDataService.GetMethodParameterValue(p.Split('.')[0], p.Split('.')[1])
                                 : p)
                    .ToArray();
            return GetMapMethodParameters(testDataMappedValues, methodInfo);
        }

        private object[] GetMapMethodParameters(object[] positionalParameters, MethodBase methodInfo)
        {
            var p = methodInfo.GetParameters();
            var methodParams = new object[p.Length];

            //add positional params
            for (var i = 0; i < positionalParameters.Length; i++)
            {
                if (i < p.Length)
                {
                    //change type to match
                    var newParam = TypeDescriptor.GetConverter(typeof(String)).ConvertTo(positionalParameters[i], p[i].ParameterType);
                    methodParams[i] = newParam;
                }
            }
            //add defaults for remaining params
            for (var i = positionalParameters.Length; i < p.Length; i++)
            {
                if (p[i].HasDefaultValue)
                    methodParams[i] = p[i].DefaultValue;
                else if (!p[i].IsOptional)
                    methodParams[i] = p[i].ParameterType.IsByRef ? null : Activator.CreateInstance(p[i].ParameterType);
            }
            return methodParams;
        }
    }
}
