using System;
using System.Collections.Generic;
using System.Text;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using Runic.Agent.TestHarness.Harness;
using System.Reflection;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.FlowManagement;

namespace Runic.Agent.Core
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
            JourneyInitialiser.InitialiseJourney(_assemblyManager, journey);
            var assembly = _assemblyManager.GetAssembly(journey.AssemblyName);
            return new Person(_functionFactory, _datetimeService, assembly);
        }
    }
}
