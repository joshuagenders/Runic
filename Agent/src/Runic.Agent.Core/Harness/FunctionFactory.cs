using Runic.Agent.Core.Models;
using System.Linq;
using System.Reflection;
using System;
using Runic.Agent.Core.AssemblyManagement;

namespace Runic.Agent.Core.Harness
{
    public class FunctionFactory : IFunctionFactory
    {
        private readonly IAssemblyManager _assemblyManager;

        public FunctionFactory(IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;
        }

        //todo think about state lifecycle for functions and assemblies
        public FunctionHarness CreateFunction(Step step, TestContext testContext)
        {
            object instance = null;
            var assembly = _assemblyManager.GetAssembly(step.Function.AssemblyName);
            var type = assembly.GetType(step.Function.AssemblyQualifiedClassName);
            if (type == null)
            {
                return null;
            }
            instance = Activator.CreateInstance(type);
            var testContextProperties = instance.GetType()
                                                .GetProperties()
                                                .Where(p => p.GetType() == typeof(TestContext) 
                                                         && p.GetType().GetTypeInfo().IsPublic);

            if (testContextProperties.Any())
            {
                foreach (var prop in testContextProperties)
                {
                    prop.SetValue(instance, testContext);
                }
            }

            return new FunctionHarness(instance, step);
        }
    }
}
