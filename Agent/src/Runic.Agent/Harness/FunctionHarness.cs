using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Attributes;
using NLog;
using Runic.Agent.Metrics;

namespace Runic.Agent.Harness
{
    public class FunctionHarness
    {
        private object _instance { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public void Bind(object functionInstance)
        {
            _instance = functionInstance;
        }

        public async Task Execute(string functionName, CancellationToken ctx = default(CancellationToken))
        {
            await BeforeEach(functionName, ctx);

            try
            {
                await ExecuteFunction(functionName);
                Clients.Statsd?.Count($"functions.{functionName}.actions.execute.success");
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing {functionName}");
                _logger.Error(e);
                Clients.Statsd?.Count($"functions.{functionName}.actions.execute.error");
            }

            await AfterEach(functionName, ctx);
        }

        private async Task ExecuteFunction(string name)
        {
            var methods = _instance.GetType().GetRuntimeMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<FunctionAttribute>();
                if (attribute != null && attribute.Name == name)
                    await Task.Run(() => method.Invoke(_instance, null));
            }
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

        private async Task BeforeEach(string functionName, CancellationToken ctx = default(CancellationToken))
        {
            try
            {
                await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute));
                Clients.Statsd?.Count($"functions.{functionName}.actions.beforeEach.success");
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing before each for {functionName}");
                _logger.Error(e);
                Clients.Statsd?.Count($"functions.{functionName}.actions.beforeEach.error");
            }
        }

        private async Task AfterEach(string functionName, CancellationToken ctx = default(CancellationToken))
        {
            try
            {
                await ExecuteMethodWithAttribute(typeof(AfterEachAttribute));
                Clients.Statsd?.Count($"functions.{functionName}.actions.afterEach.success");
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing after each for {functionName}");
                _logger.Error(e);
                Clients.Statsd?.Count($"functions.{functionName}.actions.afterEach.error");
            }
        }
    }
}