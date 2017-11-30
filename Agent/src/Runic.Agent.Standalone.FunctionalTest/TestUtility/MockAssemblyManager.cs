using Moq;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.FunctionalTest.TestUtility
{
    public class MockAssemblyManager
    {
        public IAssemblyManager Object { get; set; }
        public MockAssemblyManager()
        {
            var assemblyManager = new Mock<IAssemblyManager>();
            assemblyManager.Setup(a => a.GetAssembly(It.IsAny<string>())).Returns(GetType().GetTypeInfo().Assembly);
            assemblyManager.Setup(a => a.GetAvailableMethods()).Returns(GetAvailableMethods());
            Object = assemblyManager.Object;
        }
        private IList<MethodInformation> GetAvailableMethods()
        {
            return GetType().GetTypeInfo()
                            .Assembly
                            .GetTypes()
                            .SelectMany(t => t.GetRuntimeMethods()
                                              .Select(SelectMethodInformation))
                            .ToList(); 
        }
        private MethodInformation SelectMethodInformation(MethodInfo methodInfo) => new MethodInformation()
        {
            AssemblyName = methodInfo.DeclaringType.GetTypeInfo().Assembly.FullName,
            AssemblyQualifiedClassName = methodInfo.DeclaringType.FullName,
            MethodName = methodInfo.Name
        };
    }
}
