using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public class FlowPatternController : IFlowPatternController
    {
        public int PollingIntervalMilliseconds { get; set; } = 100;
        private readonly IDatetimeService _datetimeService;
        private readonly IThreadManager _threadManager;
        private readonly ConcurrentDictionary<string, Flow> _runningFlows;
        private readonly ConcurrentDictionary<string, Tuple<DateTime, IThreadPattern>> _runningPatterns;
        
        public FlowPatternController(IDatetimeService datetimeService, IThreadManager threadManager)
        {
            _datetimeService = datetimeService;
            _threadManager = threadManager;
            _runningFlows = new ConcurrentDictionary<string, Flow>();
            _runningPatterns = new ConcurrentDictionary<string, Tuple<DateTime, IThreadPattern>>();
        }
        
        public List<Tuple<string,Flow>> GetRunningFlows()
        {
            return _runningFlows.Select(f => Tuple.Create(f.Key, f.Value)).ToList();
        }

        public List<Tuple<string, IThreadPattern>> GetRunningThreadPatterns()
        {
            return _runningPatterns.Select(f => Tuple.Create(f.Key, f.Value.Item2)).ToList();
        }

        public void AddThreadPattern(string id, Flow flow, IThreadPattern pattern, CancellationToken ctx)
        {
            _runningFlows.TryAdd(id, flow);
            _runningPatterns.TryAdd(id, Tuple.Create(_datetimeService.Now, pattern));
        }

        public async Task Stop(string id, CancellationToken ctx)
        {
            await _threadManager.SetThreadLevelAsync(id, _runningFlows[id], 0);
            _threadManager.StopFlow(id);
            if (!_runningFlows.TryRemove(id, out Flow flow))
            {
                throw new MemberAccessException("Failed to remove flow on stop");
            }
            if (!_runningPatterns.TryRemove(id, out Tuple<DateTime,IThreadPattern> pattern))
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
            _runningFlows.Clear();
        }

        public async Task Run(CancellationToken ctx)
        {
            while (!ctx.IsCancellationRequested)
            {
                foreach (var pattern in _runningPatterns)
                {
                    var threadCount = pattern.Value.Item2.GetCurrentThreadLevel(pattern.Value.Item1);
                    await _threadManager.SetThreadLevelAsync(pattern.Key, _runningFlows[pattern.Key], threadCount);
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
            _runningFlows.Clear();
        }
    }
}
