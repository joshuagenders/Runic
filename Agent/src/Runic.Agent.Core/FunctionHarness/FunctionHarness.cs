using Runic.Framework.Clients;
using Runic.Framework.Attributes;
using Runic.Agent.Core.Exceptions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Core.ExternalInterfaces;

namespace Runic.Agent.Core.FunctionHarness
{
    public class FunctionHarness
    {
        private readonly ILoggingHandler _log;
        private readonly IStatsClient _stats;
        private object _instance { get; set; }
        private object[] _positionalParameters { get; set; }
        private string _functionName { get; set; }
        private bool _getNextStepFromResult { get; set; }
        private string _stepName { get; set; }
        private string _nextStep { get; set; }

        public FunctionHarness(IStatsClient stats, ILoggingHandler loggingHandler)
        {
            _stats = stats;
            _log = loggingHandler;
        }

        public void Bind(object functionInstance, string stepName, string functionName, bool getNextStepFromResult, params object[] positionalParameters)
        {
            _instance = functionInstance;
            _functionName = functionName;
            _positionalParameters = positionalParameters;
            _getNextStepFromResult = getNextStepFromResult;
            _stepName = stepName;
        }

        public async Task<FunctionResult> OrchestrateFunctionExecutionAsync(CancellationToken ctx = default(CancellationToken))
        {
            FunctionResult result = new FunctionResult()
            {
                FunctionName = _functionName,
                StepName = _stepName,
                FunctionParameters = _positionalParameters
            };
            var timer = new Stopwatch();
            try
            {
                timer.Start();
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute), ctx);
                await ExecuteFunctionAsync(ctx);
                await ExecuteMethodWithAttribute(typeof(AfterEachAttribute), ctx);
                timer.Stop();
                _stats.CountFunctionSuccess(_functionName);
                result.Success = true;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
                result.NextStep = _nextStep;
            }
            catch (Exception ex)
            {
                _log.Error($"Error in function execution for step {_stepName} and function {_functionName}", ex);
                _stats.CountFunctionFailure(_functionName);
                timer.Stop();

                result.Exception = ex;
                result.Success = false;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
            }
            
            return result;
        }

        public async Task ExecuteFunctionAsync(CancellationToken ctx = default(CancellationToken))
        {
            //TODO pass overrides and use of dataservice
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
                var result = await ExecuteMethodWithReturnAsync(functionMethod, ctx, GetMapMethodParameters(_positionalParameters, functionMethod));
                _nextStep = result;
            }
            else
            {
                await ExecuteMethodAsync(functionMethod, ctx, GetMapMethodParameters(_positionalParameters, functionMethod));
            }
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c.GetType() == typeof(AsyncStateMachineAttribute));
        }

        public async Task<string> ExecuteMethodWithReturnAsync(MethodInfo method, CancellationToken ctx = default(CancellationToken), params object[] inputParams)
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

        public async Task ExecuteMethodAsync(MethodInfo method, CancellationToken ctx = default(CancellationToken), params object[] inputParams)
        {
            if (IsAsyncMethod(method))
            {
                var task = (Task)method.Invoke(_instance, inputParams);
                await task;
            }
            else
            {
                await Task.Run(() => method.Invoke(_instance, inputParams), ctx);
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

        private object[] GetMapMethodParameters(object[] positionalParameters, MethodInfo methodInfo)
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

        private async Task ExecuteMethodWithAttribute(Type attributeType, CancellationToken ctx = default(CancellationToken))
        {
            var method = GetMethodWithAttribute(attributeType);
            if (method != null)
                await ExecuteMethodAsync(method, ctx);
        }
    }
}