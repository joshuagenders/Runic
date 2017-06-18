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
        private readonly IStats _stats;
        private object[] _positionalParameters { get; set; }
        private bool _getNextStepFromResult { get; set; }
        public string NextStep { get; set; }
        public string StepName { get; set; }

        public FunctionHarness(IStats stats)
        {
            _stats = stats;
        }

        public void Bind(object functionInstance, string stepName, string functionName, bool getNextStepFromResult, params object[] positionalParameters)
        {
            _instance = functionInstance;
            _functionName = functionName;
            _positionalParameters = positionalParameters;
            _getNextStepFromResult = getNextStepFromResult;
            StepName = StepName;
        }

        public async Task<bool> OrchestrateFunctionExecutionAsync(CancellationToken ct)
        {
            try
            {
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute), ct);
                await ExecuteFunctionAsync(ct);
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

        public async Task ExecuteFunctionAsync(CancellationToken ct)
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
                    break;
                }
            }
            if (functionMethod == null)
                throw new FunctionWithAttributeNotFoundException(_functionName);
            if (_getNextStepFromResult)
            {
                var result = await ExecuteMethodWithReturnAsync(functionMethod, ct, GetMapParameters(_positionalParameters, functionMethod));
                NextStep = result;
            }
            else
            {
                await ExecuteMethodAsync(functionMethod, ct, GetMapParameters(_positionalParameters, functionMethod));
            }
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c.GetType() == typeof(AsyncStateMachineAttribute));
        }

        public async Task<string> ExecuteMethodWithReturnAsync(MethodInfo method, CancellationToken ct, params object[] inputParams)
        {
            if (IsAsyncMethod(method))
            {
                return await (Task<string>)method.Invoke(_instance, inputParams);
            }
            else
            {
                string result = (string)method.Invoke(_instance, inputParams);
                return result;
            }
        }

        public async Task ExecuteMethodAsync(MethodInfo method, CancellationToken ct, params object[] inputParams)
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

        private object[] GetMapParameters(object[] positionalParameters, MethodInfo methodInfo)
        {
            var p = methodInfo.GetParameters();
            var methodParams = new object[p.Length];

            //add positional params
            for (var i = 0; i < positionalParameters.Length; i++)
            {
                if (i < p.Length)
                    methodParams[i] = positionalParameters[i];
            }
            //add defaults for remaining params
            for (var i = positionalParameters.Length; i < p.Length; i++)
            {
                if(p[i].HasDefaultValue)
                    methodParams[i] = p[i].DefaultValue;
                else if (!p[i].IsOptional)
                    methodParams[i] = p[i].ParameterType.IsByRef ? null : Activator.CreateInstance(p[i].ParameterType);
            }
            return methodParams;
        }

        private async Task ExecuteMethodWithAttribute(Type attributeType, CancellationToken ct)
        {
            var method = GetMethodWithAttribute(attributeType);
            if (method != null)
                await ExecuteMethodAsync(method, ct);
        }
    }
}