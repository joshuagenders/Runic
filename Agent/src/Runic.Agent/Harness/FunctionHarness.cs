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
        
        public void Bind(object functionInstance, string functionName)
        {
            _instance = functionInstance;
            _functionName = functionName;
        }

        public async Task<bool> Execute(CancellationToken ct)
        {
            try
            {
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute), ct);
                await ExecuteFunction(ct);
                await ExecuteMethodWithAttribute(typeof(AfterEachAttribute), ct);
                Stats.CountFunctionSuccess(_functionName);
                return true;
            }
            catch (Exception)
            {
                Stats.CountFunctionFailure(_functionName);
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

            await ExecuteMethod(functionMethod, ct);
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c.GetType() == typeof(AsyncStateMachineAttribute));
        }

        public async Task ExecuteMethod(MethodInfo method, CancellationToken ct)
        {
            if (IsAsyncMethod(method))
            {
                //await Task.Factory.StartNew(async () => await (Task)method.Invoke(_instance, null), ct);
                var task = (Task)method.Invoke(_instance, null);
                await task;
            }
            else
            {
                await Task.Run(() => method.Invoke(_instance, null), ct);
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