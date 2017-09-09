using Autofac;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.TestUtility
{
    public abstract class TestEnvironmentBuilder : TestEnvironment
    {
        private ContainerBuilder _builder { get; set; }
        protected TestEnvironmentBuilder()
        {
            _builder = new ContainerBuilder();
            RegisterTestObjects();
        }

        public IContainer Build()
        {
            return _builder.Build();
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

            _builder.RegisterInstance(instance);
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
                _builder.RegisterInstance(instance).As(asType);
            }
        }

        public TestEnvironmentBuilder WithType<T, U>()
        {
            _builder.RegisterType<T>().As<U>();
            return this;
        }

        public TestEnvironmentBuilder WithInstance<T>(T obj) where T : class
        {
            _builder.RegisterInstance<T>(obj);
            return this;
        }

        public TestEnvironmentBuilder WithSingleInstanceType<T, U>()
        {
            _builder.RegisterType<T>().As<U>().SingleInstance();
            return this;
        }

        public virtual TestEnvironmentBuilder WithStandardTypes()
        {
            return WithSingleInstanceType<ThreadManager, IThreadManager>()
                   .WithSingleInstanceType<PatternController, IPatternController>()
                   .WithType<RunnerService, IRunnerService>()
                   .WithType<FunctionFactory, IFunctionFactory>();
        }
    }
}
