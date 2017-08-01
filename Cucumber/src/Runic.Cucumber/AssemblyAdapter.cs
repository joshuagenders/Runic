using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber
{
    public class AssemblyAdapter : IAssemblyAdapter
    {
        public readonly Assembly _assembly;
        public readonly TestStateManager _stateManager;

        public List<MethodInfo> Methods { get; set; }
        public AssemblyAdapter(Assembly assembly, TestStateManager stateManager)
        {
            _assembly = assembly;
            _stateManager = stateManager;
            RegisterAttributes();
        }

        public async Task ExecuteMethodAsync(object instance, MethodInfo method, CancellationToken ctx, params object[] inputParams)
        {
            if (IsAsyncMethod(method))
            {
                var task = (Task)method.Invoke(instance, GetMapMethodParameters(inputParams, method));
                await task;
            }
            else
            {
                await Task.Run(() => method.Invoke(instance, GetMapMethodParameters(inputParams, method)), ctx);
            }
        }

        public async Task ExecuteMethodFromStatementAsync(string statement, object[] arguments, CancellationToken ctx = default(CancellationToken))
        {
            var method = GetMethodTypeFromStatement(statement);
            var instance = _stateManager.GetObject(method.DeclaringType);

            await ExecuteMethodAsync(instance, method, ctx, arguments);
        }

        private MethodInfo GetMethodTypeFromStatement(string statement, CancellationToken ctx = default(CancellationToken))
        {
            foreach (var method in Methods)
            {
                var attribute = method.GetCustomAttributes().First(a => a.GetType()
                                                            .GetInterfaces()
                                                            .Contains(typeof(IRegexMatchable))) as IRegexMatchable;
                var pattern = attribute.GetMatchString;
                if (Regex.Match(statement, pattern).Success)
                    return method;
            }

            throw new MethodNotFoundException($"Method not found for statement: {statement}");
        }

        public void RegisterAttributes()
        {
            var assemblymethods = _assembly.GetTypes()
                                           .SelectMany(t => t.GetMethods());

            Methods = assemblymethods.Where(m => m.GetCustomAttributes()
                                                  .Any(a => a.GetType()
                                                             .GetInterfaces()
                                                             .Contains(typeof(IRegexMatchable))))
                                     .ToList();
        }
        
        private bool IsAsyncMethod(MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof(Task)) ||
                        method.ReturnTypeCustomAttributes
                              .GetCustomAttributes(false)
                              .Any(c => c.GetType() == typeof(AsyncStateMachineAttribute));
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
                if (p[i].HasDefaultValue)
                    methodParams[i] = p[i].DefaultValue;
                else if (!p[i].IsOptional)
                    methodParams[i] = p[i].ParameterType.IsByRef ? null : Activator.CreateInstance(p[i].ParameterType);
            }
            return methodParams;
        }
    }
}
