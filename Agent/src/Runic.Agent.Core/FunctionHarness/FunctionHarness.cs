using Runic.Agent.Core.Exceptions;
using Runic.Agent.Core.Services;
using Runic.Framework.Attributes;
using Runic.Framework.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.FunctionHarness
{
    public class FunctionHarness
    {
        private readonly IDataService _dataService;
        private readonly IEventService _eventService;

        private object _instance { get; set; }
        private Step _step { get; set; }
        private string _nextStep { get; set; }

        public FunctionHarness(IEventService eventService, IDataService dataService)
        {
            _eventService = eventService;
            _dataService = dataService;
        }

        public void Bind(object functionInstance, Step step)
        {
            _instance = functionInstance;
            _step = step;
        }

        public async Task<FunctionResult> OrchestrateFunctionExecutionAsync(CancellationToken ctx = default(CancellationToken))
        {
            FunctionResult result = new FunctionResult()
            {
                FunctionName = _step.Function.FunctionName,
                StepName = _step.StepName
            };
            var timer = new Stopwatch();
            try
            {
                timer.Start();
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute), ctx);
                await ExecuteFunctionAsync(ctx);
                await ExecuteMethodWithAttribute(typeof(AfterEachAttribute), ctx);
                timer.Stop();
                result.Success = true;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
                result.NextStep = _nextStep;
            }
            catch (Exception ex)
            {
                _eventService.Error($"Error in function execution for step {_step?.StepName} and function {_step?.Function?.FunctionName}", ex);
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
                if (attribute != null && attribute.Name == _step.Function.FunctionName)
                {
                    functionMethod = method;
                    break;
                }
            }
            if (functionMethod == null)
                throw new FunctionWithAttributeNotFoundException(_step.Function.FunctionName);
            if (_step.Function.GetNextStepFromFunctionResult)
            {
                var parameters = _dataService.GetParams(_step.Function.Parameters?.ToArray(), functionMethod);
                var result = await ExecuteMethodWithReturnAsync(functionMethod, ctx, parameters);
                _nextStep = result;
            }
            else
            {
                var parameters = _dataService.GetParams(_step.Function.Parameters?.ToArray(), functionMethod);
                await ExecuteMethodAsync(functionMethod, ctx, parameters);
            }
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c is AsyncStateMachineAttribute);
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
                await (Task)method.Invoke(_instance, inputParams);
            }
            else
            {
                method.Invoke(_instance, inputParams);
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

        private async Task ExecuteMethodWithAttribute(Type attributeType, CancellationToken ctx = default(CancellationToken))
        {
            var method = GetMethodWithAttribute(attributeType);
            if (method != null)
                await ExecuteMethodAsync(method, ctx);
        }
    }
}