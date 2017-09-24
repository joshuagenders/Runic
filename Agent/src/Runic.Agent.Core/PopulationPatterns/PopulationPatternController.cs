using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadPatterns
{
    public class PopulationPatternController : IPopulationPatternController
    {
        public int PollingIntervalMilliseconds { get; set; } = 100;
        private readonly IDatetimeService _datetimeService;
        private readonly IPopulationControl _threadManager;
        private readonly ConcurrentDictionary<string, Journey> _runningJourneys;
        private readonly ConcurrentDictionary<string, Tuple<DateTime, IPopulationPattern>> _runningPatterns;
        
        public PopulationPatternController(IDatetimeService datetimeService, IPopulationControl threadManager)
        {
            _datetimeService = datetimeService;
            _threadManager = threadManager;
            _runningJourneys = new ConcurrentDictionary<string, Journey>();
            _runningPatterns = new ConcurrentDictionary<string, Tuple<DateTime, IPopulationPattern>>();
        }
        
        public List<Tuple<string, Journey>> GetRunningFlows()
        {
            return _runningJourneys.Select(f => Tuple.Create(f.Key, f.Value)).ToList();
        }

        public List<Tuple<string, IPopulationPattern>> GetRunningThreadPatterns()
        {
            return _runningPatterns.Select(f => Tuple.Create(f.Key, f.Value.Item2)).ToList();
        }

        public void AddPopulationPattern(string id, Journey flow, IPopulationPattern pattern, CancellationToken ctx)
        {
            _runningJourneys.TryAdd(id, flow);
            _runningPatterns.TryAdd(id, Tuple.Create(_datetimeService.Now, pattern));
        }

        public async Task Stop(string id, CancellationToken ctx)
        {
            await _threadManager.SetThreadLevelAsync(id, _runningJourneys[id], 0);
            _threadManager.StopFlow(id);
            if (!_runningJourneys.TryRemove(id, out Journey flow))
            {
                throw new MemberAccessException("Failed to remove flow on stop");
            }
            if (!_runningPatterns.TryRemove(id, out Tuple<DateTime,IPopulationPattern> pattern))
            {
                throw new MemberAccessException("Failed to remove pattern on stop");
            }
        }

        public async Task StopAll(CancellationToken ctx)
        {
            foreach (var id in _runningPatterns.Keys)
            {
                await Stop(id, ctx);
            }
            await _threadManager.CancelAll();
            _runningPatterns.Clear();
            _runningJourneys.Clear();
        }

        public async Task Run(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested)
            {
                foreach (var pattern in _runningPatterns)
                {
                    var threadCount = pattern.Value.Item2.GetCurrentActivePopulationCount(pattern.Value.Item1);
                    await _threadManager.SetThreadLevelAsync(pattern.Key, _runningJourneys[pattern.Key], threadCount);
                    if (threadCount == 0)
                    {
                        await Stop(pattern.Key, ctx);   
                    }
                }
                await _datetimeService.WaitMilliseconds(PollingIntervalMilliseconds);
                if (_runningPatterns.Count == 0)
                {
                    break;
                }
            }
            _runningPatterns.Clear();
            _runningJourneys.Clear();
        }
    }
}
