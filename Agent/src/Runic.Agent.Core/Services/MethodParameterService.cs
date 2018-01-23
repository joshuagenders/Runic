using System;
using System.Reflection;

namespace Runic.Agent.Core.Services
{
    public class MethodParameterService
    {
        public object[] GetParams(string[] positionalParameters, MethodInfo methodInfo)
        {
            var p = methodInfo.GetParameters();
            if (p == null)
            {
                return new object[] { };
            }
            var methodParams = new object[p.Length];

            int mappedParamCount = positionalParameters == null ? 0 : positionalParameters.Length;
            if (positionalParameters != null)
            {
                //add positional params
                for (var i = 0; i < mappedParamCount; i++)
                {
                    if (i < p.Length)
                    {
                        //change type to match
                        methodParams[i] = Convert.ChangeType(positionalParameters[i], p[i].ParameterType);
                    }
                }
            }
            //add defaults for remaining params
            for (var i = mappedParamCount; i < p.Length; i++)
            {
                if (p[i].HasDefaultValue)
                    methodParams[i] = p[i].DefaultValue;
                else if (!p[i].IsOptional)
                    methodParams[i] = p[i].ParameterType.IsValueType ? Activator.CreateInstance(p[i].ParameterType) : null;
            }
            return methodParams;
        }
    }
}