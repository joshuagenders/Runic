using Runic.Agent.Core.ThreadManagement;
using Runic.Agent.Core.ThreadPatterns;
using Runic.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public class PatternController : IPatternController
    {
        public int PollingIntervalMilliseconds { get; set; } = 100;
        private readonly IDatetimeService _datetimeService;
        private readonly IThreadManager _threadManager;
        private readonly ConcurrentDictionary<string, Flow> _runningFlows;
        private readonly ConcurrentDictionary<string, Tuple<DateTime, IThreadPattern>> _runningPatterns;
        
        public PatternController(IDatetimeService datetimeService, IThreadManager threadManager)
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

        public void StartThreadPattern(string id, Flow flow, IThreadPattern pattern, CancellationToken ctx)
        {
            _runningFlows.TryAdd(id, flow);
            _runningPatterns.TryAdd(id, Tuple.Create(_datetimeService.Now, pattern));
        }

        public async Task StopFlow(Flow flow, CancellationToken ctx)
        {
            var ids = _runningFlows.Where(f => f.Value == flow).Select(f=> f.Key);
            foreach (var id in ids)
            {
                await Stop(id, ctx);
            }
        }

        public async Task StopThreadPattern(IThreadPattern threadPattern, CancellationToken ctx)
        {
            var ids = _runningPatterns.Where(p => p.Value == threadPattern).Select(p => p.Key);
            foreach (var id in ids)
            {
                await Stop(id, ctx);
            }
        }

        public async Task Stop(string id, CancellationToken ctx)
        {
            await _threadManager.SetThreadLevelAsync(id, _runningFlows[id], 0);
            _threadManager.StopFlow(id);
            _runningFlows.TryRemove(id, out Flow flow);
            _runningPatterns.TryRemove(id, out Tuple<DateTime,IThreadPattern> pattern);
        }

        public async Task StopAll(CancellationToken ctx)
        {
            foreach (var id in _runningPatterns.Keys)
            {
                await Stop(id, ctx);
            }
            await _threadManager.CancelAll();
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
