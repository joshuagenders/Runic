using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Runic.Core.Attributes;
using NLog;

namespace Runic.Agent.Harness
{
    public class FunctionHarness : IFunctionHarness
    {
        private object _instance { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Bind(object functionInstance)
        {
            _instance = functionInstance;
        }
        
        public async Task Execute(string functionName, CancellationToken ctx = default(CancellationToken))
        {
            try
            {
                await BeforeEach();
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing before each for {functionName}");
                _logger.Error(e);
            }
            try
            {
                await ExecuteFunction(functionName);
                if (ctx.IsCancellationRequested)
                    return;
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing {functionName}");
                _logger.Error(e);
            }

            try
            {
                await AfterEach();
            }
            catch (Exception e)
            {
                _logger.Error($"Error executing {functionName}");
                _logger.Error(e);
            }
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

        private async Task BeforeEach()
        {
            await ExecuteMethodWithAttribute(typeof(BeforeEachAttribute));
        }

        private async Task AfterEach()
        {
            await ExecuteMethodWithAttribute(typeof(AfterEachAttribute));
        }
    }
}