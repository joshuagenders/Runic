using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.TestHarness.Services
{
    internal class MethodParameterService
    {
        public object[] GetParams(string[] positionalParameters, MethodInfo methodInfo)
        {
            if (positionalParameters == null || !positionalParameters.Any())
            {
                return new object[] { };
            }
            return GetMapMethodParameters(positionalParameters, methodInfo);
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
                    methodParams[i] = Convert.ChangeType(positionalParameters[i], p[i].ParameterType); 
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