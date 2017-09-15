using Runic.Agent.Framework.Models;
using System.Linq;
using System.Reflection;
using System;
using Runic.Agent.TestHarness.Services;

namespace Runic.Agent.TestHarness.Harness
{
    public class FunctionFactory : IFunctionFactory
    {
        private readonly IDataService _dataService;
        private readonly Assembly _assembly;

        public FunctionFactory(Assembly assembly, IDataService dataService)
        {
            _dataService = dataService;
            _assembly = assembly;
        }

        //todo think about state lifecycle for functions and assemblies
        public FunctionHarness CreateFunction(Step step, TestContext testContext)
        {
            object instance = null;
            try
            {
                var type = _assembly.GetType(step.Function.AssemblyQualifiedClassName);
                if (type == null)
                {
                    return null;
                }
                instance = Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new ClassNotFoundInAssemblyException($"Could not locate {step.Function.AssemblyQualifiedClassName}", ex);
            }
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

            var harness = new FunctionHarness(_dataService);
            harness.Bind(instance, step);
            return harness;
        }
    }
}
