using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.TaskManagement
{
    public class JourneyTask : AbstractTask
    {
        private readonly Journey _journey;
        private readonly IPerson _person;
        private readonly CancellationTokenSource _cts;
        private readonly IPersonAttributeService _personAttributeService;
        private readonly IPopulationController _populationController;

        private Task _journeyTask { get; set; }

        public JourneyTask(
            Journey journey, 
            IPersonFactory personFactory, 
            IPersonAttributeService personAttributeService,
            IPopulationController populationController)
        {
            _person = personFactory.GetPerson(journey);
            _journey = journey;
            _personAttributeService = personAttributeService;
            _cts = new CancellationTokenSource();
            _populationController = populationController;
        }

        public override void Run()
        {
            Dictionary<string, string> attributes = null;
            try
            {
                //go sync here?
                _populationController.RequestPerson().Wait();
                attributes = _personAttributeService.RequestAttributes();
                _person.SetAttributes(attributes);
                _journeyTask = _person.PerformJourneyAsync(_journey, _cts.Token);
            }
            finally
            {
                _personAttributeService.ReleaseAttributes(attributes);
                _populationController.ReleasePerson();
            }
            
            if (_journeyTask.IsFaulted)
                Fail();
            else
                Succeed();
        }
    }
}
