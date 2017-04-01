using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Attributes;
using NLog;
using Runic.Agent.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Runic.Agent.Harness
{
    public class FunctionHarness
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private object _instance { get; set; }
        private string _functionName { get; set; }
        private IStats _stats { get; set; }
        private object[] _inputParams { get; set; }

        public FunctionHarness(IStats stats)
        {
            _stats = stats;
        }

        public void Bind(object functionInstance, string functionName, params object[] inputParameters)
        {
            _instance = functionInstance;
            _functionName = functionName;
            _inputParams = inputParameters;
        }

        public async Task<bool> Execute(CancellationToken ct)
        {
            try
            {
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute), ct);
                await ExecuteFunction(ct);
                await ExecuteMethodWithAttribute(typeof(AfterEachAttribute), ct);
                _stats.CountFunctionSuccess(_functionName);
                return true;
            }
            catch (Exception e)
            {
                _stats.CountFunctionFailure(_functionName);
                return false;
            }
        }

        public async Task ExecuteFunction(CancellationToken ct)
        {
            //todo pass overrides
            var methods = _instance.GetType().GetRuntimeMethods();
            MethodInfo functionMethod = null;
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<FunctionAttribute>();
                if (attribute != null && attribute.Name == _functionName)
                {
                    functionMethod = method;
                }
            }
            if (functionMethod == null)
                throw new FunctionWithAttributeNotFoundException(_functionName);

            await ExecuteMethod(functionMethod, ct, _inputParams);
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c.GetType() == typeof(AsyncStateMachineAttribute));
        }

        public async Task ExecuteMethod(MethodInfo method, CancellationToken ct, params object[] inputParams)
        {
            if (IsAsyncMethod(method))
            {
                var task = (Task)method.Invoke(_instance, inputParams);
                await task;
            }
            else
            {
                await Task.Run(() => method.Invoke(_instance, inputParams), ct);
            }
        }
        
        public MethodInfo GetMethodWithAttribute(Type attributeType)
        {
            var methods = _instance.GetType().GetMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute(attributeType);
                if (attribute != null)
                    return method;
            }
            return null;
        }

        private async Task ExecuteMethodWithAttribute(Type attributeType, CancellationToken ct)
        {
            var method = GetMethodWithAttribute(attributeType);
            if (method != null)
                await ExecuteMethod(method, ct);
        }
    }
}