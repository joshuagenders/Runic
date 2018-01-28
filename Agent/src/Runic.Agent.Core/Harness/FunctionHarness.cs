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
        public async Task<Result> ExecuteTestAsync(Assembly assembly, Step step)
        {
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
                await ExecuteStepAsync(instance, step);
                timer.Stop();
                return new Result(true, timer.ElapsedMilliseconds, null, step);
            }
            catch (Exception ex)
            {
                timer.Stop();
                return new Result(false, timer.ElapsedMilliseconds, ex.Message, step);
            }
        }

        private async Task ExecuteStepAsync(object instance, Step step)
        {
            var functionMethods =
                instance.GetType()
                         .GetRuntimeMethods()
                         .Where(m => m.Name == step.Function.MethodName);

            if (functionMethods.Any())
                throw new ArgumentException($"Method {step.Function.MethodName} not found on type {instance.GetType().Name}.");

            var method = functionMethods.First();
            var parameters = new MethodParameterService().GetParams(step.Function.PositionalMethodParameterValues?.ToArray(), method);
            if (IsAsyncMethod(method))
            {
                await ((Task)method.Invoke(instance, parameters));
            }
            else
            {
                method.Invoke(instance, parameters);
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