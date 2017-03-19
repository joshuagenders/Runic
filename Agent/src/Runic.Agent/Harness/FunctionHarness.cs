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
        private object _instance { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private string _functionName { get; set; }
        public string Status { get; set; }
        public void Bind(object functionInstance, string functionName)
        {
            _instance = functionInstance;
            _functionName = functionName;
            Status = "FunctionBound";
        }

        public async Task Execute(CancellationToken ctx = default(CancellationToken))
        {
            await BeforeEach(ctx);

            try
            {
                await ExecuteFunction();
                Clients.Statsd?.Count($"functions.{_functionName}.actions.execute.success");
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                Status = "Failure";
                _logger.Error($"Error executing {_functionName}");
                _logger.Error(e);
                Clients.Statsd?.Count($"functions.{_functionName}.actions.execute.error");
            }

            await AfterEach(ctx);
        }

        private async Task ExecuteFunction()
        {
            //todo pass overrides
            Status = "ExecuteFunction";
            var methods = _instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<FunctionAttribute>();
                if (attribute != null && attribute.Name == _functionName)
                {
                    if (IsAsyncMethod(method))
                    {
                        var task = (Task)method.Invoke(_instance, null);
                        await task;
                    }
                    else
                    {
                        await Task.Run(() => method.Invoke(_instance, null));
                    }
                }
            }
        }

        private bool IsAsyncMethod (MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c.GetType() == typeof(AsyncStateMachineAttribute));
        }

        private async Task ExecuteMethodWithAttribute(Type attributeType)
        {
            var methods = _instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute(attributeType);
                if (attribute != null)
                    await Task.Run(() => method.Invoke(_instance, null));
            }
        }

        private async Task BeforeEach(CancellationToken ctx = default(CancellationToken))
        {
            Status = "BeforeEach";
            try
            {
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute));
                Clients.Statsd?.Count($"functions.{_functionName}.actions.beforeEach.success");
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing before each for {_functionName}");
                _logger.Error(e);
                Clients.Statsd?.Count($"functions.{_functionName}.actions.beforeEach.error");
            }
        }

        private async Task AfterEach(CancellationToken ctx = default(CancellationToken))
        {
            Status = "AfterEach";
            try
            {
                await ExecuteMethodWithAttribute(typeof(AfterEachAttribute));
                Status = "Complete";
                Clients.Statsd?.Count($"functions.{_functionName}.actions.afterEach.success");
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing after each for {_functionName}");
                _logger.Error(e);
                Clients.Statsd?.Count($"functions.{_functionName}.actions.afterEach.error");
            }
        }
    }
}