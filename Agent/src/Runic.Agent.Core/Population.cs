using Runic.Agent.Core.Configuration;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class Population : IPopulation
    {
        private readonly IPersonAttributeService _personAttributeService;
        private readonly SemaphoreSlim Semaphore;
        public readonly int MaxPopulation;
        private readonly ICoreConfiguration _config;

        public Population(IPersonAttributeService personAttributeService, ICoreConfiguration config)
        {
            _personAttributeService = personAttributeService;
            _config = config;
            MaxPopulation = config.MaxActivePopulation;
            Semaphore = new SemaphoreSlim(MaxPopulation);
        }

        public async Task<bool> RequestPerson()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(_config.PopulationRequestTimeoutSeconds * 1000);
            try
            {
                await Semaphore.WaitAsync(cts.Token);
                return true;
            }
            catch (TaskCanceledException)
            {
                //stats, log or raise event, observer etc
                return false;
            }
        }

        public void ReleasePerson()
        {
            Semaphore.Release();
        }

        public async Task ActivatePerson(
            IPerson person, 
            Journey journey, 
            CancellationToken ctx = default(CancellationToken))
        {
            Dictionary<string, string> attributes = null;
            if (await RequestPerson())
            {
                try
                {
                    attributes = _personAttributeService.RequestAttributes();
                    person.SetAttributes(attributes);
                    await person.PerformJourneyAsync(journey, ctx);
                }
                finally
                {
                    _personAttributeService.ReleaseAttributes(attributes);
                    ReleasePerson();
                }
            }
        }
    }
}
