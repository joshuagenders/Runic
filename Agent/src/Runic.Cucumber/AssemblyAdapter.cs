using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

        public async Task ExecuteMethodFromStatementAsync(string statement, object[] arguments)
        {
            var methodDetails = GetMethodTypeFromStatement(statement);
            var instance = _stateManager.GetObject(methodDetails.Item1.DeclaringType);
            //todo append, replace, error?
            List<object> methodArgs = arguments.ToList();
            methodArgs.AddRange(methodDetails.Item2);

            await ExecuteMethodAsync(instance, methodDetails.Item1, methodArgs.ToArray());
        }

        public async Task ExecuteMethodAsync(object instance, MethodInfo method, object[] arguments)
        {
            Task task;
            if (IsAsyncMethod(method))
            {
                task = (Task)method.Invoke(instance, GetMapMethodParameters(arguments, method));
                await task;
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            }
            else
            {
                method.Invoke(instance, GetMapMethodParameters(arguments, method));
            }
        }

        public void ExecuteMethod(object instance, MethodInfo method, object[] arguments)
        {
            Task task;
            if (IsAsyncMethod(method))
            {
                task = (Task)method.Invoke(instance, GetMapMethodParameters(arguments, method));
                task.ConfigureAwait(false).GetAwaiter().GetResult();
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            }
            else
            {
                method.Invoke(instance, GetMapMethodParameters(arguments, method));
            }   
        }

        public void ExecuteMethodFromStatement(string statement, object[] arguments)
        {
            var methodDetails = GetMethodTypeFromStatement(statement);
            var instance = _stateManager.GetObject(methodDetails.Item1.DeclaringType);
            //todo append, replace, error?
            List<object> methodArgs = arguments.ToList();
            methodArgs.AddRange(methodDetails.Item2);

            ExecuteMethod(instance, methodDetails.Item1, methodArgs.ToArray());
        }

        private Tuple<MethodInfo, List<string>> GetMethodTypeFromStatement(string statement)
        {
            var matches = new List<Tuple<MethodInfo, List<string>>>();
            foreach (var method in Methods)
            {
                var attribute = method.GetCustomAttributes().Single(a => a.GetType()
                                                                          .GetInterfaces()
                                                                          .Contains(typeof(IRegexMatchable))) 
                                                            as IRegexMatchable;
                var pattern = attribute.GetMatchString;
                var inputArguments = new List<string>();
                var match = Regex.Match(statement, pattern);
                if (match.Success)
                {
                    for(var i = 1; i < match.Groups.Count; i++)
                    {
                        inputArguments.Add(match.Groups[i].Value);
                    }
                    matches.Add(Tuple.Create(method, inputArguments));
                }
            }

            if (!matches.Any())
            {
                throw new MethodNotFoundException($"Method not found for statement: {statement}");
            }
            if (matches.Count > 1)
            {
                throw new MultipleMethodsFoundException(
                    $"Multiple methods found for statement: {statement}" + 
                    string.Join(",", matches.Select(m => 
                        m.Item1.DeclaringType.FullName + "." + m.Item1.Name)));
            }
            return matches.Single();
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
                              .Any(c => c is AsyncStateMachineAttribute);
        }

        private object[] GetMapMethodParameters(object[] positionalParameters, MethodBase methodInfo)
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
                    methodParams[i] = p[i].ParameterType == typeof(string) ? "" : Activator.CreateInstance(p[i].ParameterType);
            }
            return methodParams;
        }
    }
}
