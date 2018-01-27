using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class FunctionHarness
    {
        public FunctionResult ExecuteTest(Assembly assembly, Step step)
        {
            FunctionResult result = new FunctionResult()
            {
                MethodName = step.Function.MethodName,
                StepName = step.StepName,
                Step = step
            };
            var timer = new Stopwatch();
            try
            {
                var type = assembly.GetType(step.Function.AssemblyQualifiedClassName);
                if (type == null)
                {
                    throw new ArgumentException($"Class not found for step {step.StepName}");
                }
                var instance = Activator.CreateInstance(type);

                timer.Start();
                ExecuteFunction(instance, step);
                timer.Stop();

                result.Success = true;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
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

        private void ExecuteFunction(object instance, Step step)
        {
            var functionMethod = 
                instance.GetType()
                         .GetRuntimeMethods()
                         .Where(m => m.Name == step.Function.MethodName);

            if (!functionMethod.Any())
                throw new ArgumentException($"Method {step.Function.MethodName} not found on type {instance.GetType().Name}.");

            var parameters = new MethodParameterService()
                                    .GetParams(
                                        step.Function.PositionalMethodParameterValues?.ToArray(), 
                                        functionMethod.First());
            if (IsAsyncMethod(functionMethod.First()))
            {
                ((Task)functionMethod.First().Invoke(instance, parameters))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                functionMethod.First().Invoke(instance, parameters);
            }
        }

        public async Task<Result> ExecuteTestAsync(Assembly assembly, Step step)
        {
            FunctionResult result = new FunctionResult()
            {
                MethodName = step.Function.MethodName,
                StepName = step.StepName,
                Step = step
            };
            var timer = new Stopwatch();
            try
            {
                var type = assembly.GetType(step.Function.AssemblyQualifiedClassName);
                if (type == null)
                {
                    throw new ArgumentException($"Class not found for step {step.StepName}");
                }
                var instance = Activator.CreateInstance(type);

                timer.Start();
                await ExecuteFunctionAsync(instance, step);
                timer.Stop();

                result.Success = true;
                result.ExecutionTimeMilliseconds = timer.ElapsedMilliseconds;
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

        private async Task ExecuteFunctionAsync(object instance, Step step)
        {
            var functionMethod =
                instance.GetType()
                         .GetRuntimeMethods()
                         .Where(m => m.Name == step.Function.MethodName);

            if (!functionMethod.Any())
                throw new ArgumentException($"Method {step.Function.MethodName} not found on type {instance.GetType().Name}.");

            var parameters = new MethodParameterService()
                                    .GetParams(
                                        step.Function.PositionalMethodParameterValues?.ToArray(),
                                        functionMethod.First());
            if (IsAsyncMethod(functionMethod.First()))
            {
                await ((Task)functionMethod.First().Invoke(instance, parameters));
            }
            else
            {
                functionMethod.First().Invoke(instance, parameters);
            }
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c is AsyncStateMachineAttribute);
        }
    }
}