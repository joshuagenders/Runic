using Microsoft.Extensions.DependencyInjection;
using Runic.Agent.Core.IoC;
using System;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public abstract class TestEnvironmentBuilder : TestEnvironment
    {
        private ServiceCollection _serviceCollection { get; set; }
        protected TestEnvironmentBuilder()
        {
            _serviceCollection = new ServiceCollection();
            RegisterTestObjects();
        }

        public IServiceProvider Build()
        {
            return _serviceCollection.BuildServiceProvider();
        }

        public TestEnvironmentBuilder With<T>(T instance) where T : class
        {
            var props = GetType().GetTypeInfo()
                                 .GetProperties();

            var prop = props.Where(p => p.PropertyType == typeof(TestObject<T>) &&
                                        p.PropertyType.GenericTypeArguments[0] == typeof(T));
            if (prop.Any())
            {
                var testObj = prop.First().GetValue(this);
                testObj.GetType().GetProperty("Instance").SetValue(testObj, instance);
            }

            _serviceCollection.AddSingleton(instance);
            return this;
        }

        public void RegisterTestObjects()
        {
            var props = GetType().GetTypeInfo()
                                 .GetProperties();
            var genericProps = props.Where(p => p.PropertyType.IsConstructedGenericType);
            var testObjects = genericProps.Where(p => p.PropertyType.GetGenericTypeDefinition().Equals(typeof(TestObject<>)))
                                          .ToList();

            foreach (var prop in testObjects)
            {
                var testObject = prop.GetValue(this);
                var instance = testObject.GetType().GetProperty("Instance").GetValue(testObject);
                var asType = prop.PropertyType.GenericTypeArguments[0];
                _serviceCollection.AddSingleton(asType, instance);
            }
        }

        public TestEnvironmentBuilder WithType<TService, TImplementation>() where TImplementation : class, TService, new() where TService : class
        {
            _serviceCollection.AddTransient<TService, TImplementation>();
            return this;
        }

        public TestEnvironmentBuilder WithInstance<T>(T obj) where T : class
        {
            _serviceCollection.AddSingleton(obj);
            return this;
        }

        public virtual TestEnvironmentBuilder WithStandardTypes()
        {
            CoreServiceCollection.ConfigureServices(_serviceCollection);
            return this;
        }
    }
}
