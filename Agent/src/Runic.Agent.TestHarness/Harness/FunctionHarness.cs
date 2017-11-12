﻿using Runic.Agent.Framework.Attributes;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.TestHarness.Harness
{
    public class FunctionHarness
    {
        private readonly MethodParameterService _methodParameterService;

        private object _instance { get; set; }
        private Step _step { get; set; }
        private string _nextStep { get; set; }

        public FunctionHarness()
        {
            _methodParameterService = new MethodParameterService();
        }

        public void Bind(object functionInstance, Step step)
        {
            _instance = functionInstance;
            _step = step;
        }

        public async Task<FunctionResult> ExecuteAsync(CancellationToken ctx = default(CancellationToken))
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
                timer.Stop();
                result.Exception = ex;
                result.Success = false;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
            }
            
            return result;
        }

        private async Task ExecuteFunctionAsync(CancellationToken ctx = default(CancellationToken))
        {
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
                throw new ArgumentOutOfRangeException($"Function {_step.Function.FunctionName} not found.");
            if (_step.Function.GetNextStepFromFunctionResult)
            {
                var parameters = _methodParameterService.GetParams(_step.Function.PositionalMethodParameterValues?.ToArray(), functionMethod);
                var result = await ExecuteMethodWithReturnAsync(functionMethod, ctx, parameters);
                _nextStep = result;
            }
            else
            {
                var parameters = _methodParameterService.GetParams(_step.Function.PositionalMethodParameterValues?.ToArray(), functionMethod);
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

        private async Task ExecuteMethodAsync(MethodInfo method, CancellationToken ctx, params object[] inputParams)
        {
            if (IsAsyncMethod(method))
            {
                await Task.Run(() => (Task)method.Invoke(_instance, inputParams), ctx);
            }
            else
            {
                method.Invoke(_instance, inputParams);
            }
        }
        
        private MethodInfo GetMethodWithAttribute(Type attributeType)
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