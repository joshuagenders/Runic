using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class FunctionHarness
    {
        private readonly MethodParameterService _methodParameterService;

        private readonly object _instance;
        private readonly Step _step;
        private string _nextStep { get; set; }
        private readonly SemaphoreSlim _semaphore;

        public FunctionHarness(object functionInstance, Step step)
        {
            _instance = functionInstance;
            _step = step;
            _methodParameterService = new MethodParameterService();
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<FunctionResult> ExecuteAsync(CancellationToken ctx = default(CancellationToken))
        {
            await _semaphore.WaitAsync();
            FunctionResult result = new FunctionResult()
            {
                MethodName = _step.Function.MethodName,
                StepName = _step.StepName,
                Step = _step
            };
            var timer = new Stopwatch();
            try
            {
                timer.Start();
                await ExecuteFunctionAsync(_step.Function.MethodName, ctx);
                timer.Stop();
                result.Success = true;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
                result.NextStep = _nextStep;
            }
            catch (Exception ex)
            {
                timer.Stop();
                result.Exception = ex;
                result.Success = false;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
            }
            _semaphore.Release();
            return result;
        }

        private async Task ExecuteFunctionAsync(string methodName, CancellationToken ctx = default(CancellationToken))
        {
            var functionMethod = 
                _instance.GetType()
                         .GetRuntimeMethods()
                         .Where(m => m.Name == methodName);

            if (!functionMethod.Any())
                throw new ArgumentException($"Method {methodName} not found on type {_instance.GetType().Name}.");

            if (_step.Function.GetNextStepFromMethodResult)
            {
                var parameters = _methodParameterService.GetParams(_step.Function.PositionalMethodParameterValues?.ToArray(), functionMethod.First());
                var result = await ExecuteMethodWithReturnAsync(functionMethod.First(), ctx, parameters);
                _nextStep = result;
            }
            else
            {
                var parameters = _methodParameterService.GetParams(_step.Function.PositionalMethodParameterValues?.ToArray(), functionMethod.First());
                await ExecuteMethodAsync(functionMethod.First(), parameters);
            }
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c is AsyncStateMachineAttribute);
        }

        private async Task<string> ExecuteMethodWithReturnAsync(MethodInfo method, params object[] inputParams)
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

        private async Task ExecuteMethodAsync(MethodInfo method, params object[] inputParams)
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
    }
}