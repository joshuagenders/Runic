using Runic.Agent.Framework.Models;
using System.Linq;
using System.Reflection;
using System;
using Runic.Agent.TestHarness.Services;

namespace Runic.Agent.TestHarness.Harness
{
    public class FunctionFactory : IFunctionFactory
    {
        private readonly Assembly _assembly;

        public FunctionFactory(Assembly assembly)
        {
            _assembly = assembly;
        }

        //todo think about state lifecycle for functions and assemblies
        public FunctionHarness CreateFunction(Step step, TestContext testContext)
        {
            object instance = null;
            var type = _assembly.GetType(step.Function.AssemblyQualifiedClassName);
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
