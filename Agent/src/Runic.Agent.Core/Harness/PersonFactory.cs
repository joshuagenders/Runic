using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.Harness
{
    public class PersonFactory : IPersonFactory
    {
        private readonly IDatetimeService _datetimeService;
        private readonly IFunctionFactory _functionFactory;
        private readonly IAssemblyManager _assemblyManager;

        public PersonFactory(IFunctionFactory functionFactory, IDatetimeService datetimeService, IAssemblyManager assemblyManager)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _assemblyManager = assemblyManager;
        }
        public IPerson GetPerson(Journey journey)
        {
            return new Person(_functionFactory, _datetimeService, _assemblyManager);
        }
    }
}
