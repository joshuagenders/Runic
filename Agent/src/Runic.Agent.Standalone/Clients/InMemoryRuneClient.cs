using System;
using System.Threading.Tasks;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace Runic.Agent.Standalone.Clients
{
    public class InMemoryRuneClient : IRuneClient
    {
        public ConcurrentDictionary<Type, ConcurrentQueue<RuneWrapper>> RuneQueues { get; set; }
        public int MaxQueueSize { get; set; }
        public int CurrentQueueSize => RuneQueues.Sum(q => q.Value.Count);
        public InMemoryRuneClient(int maxQueueSize = 1024)
        {
            RuneQueues = new ConcurrentDictionary<Type, ConcurrentQueue<RuneWrapper>>();
            MaxQueueSize = maxQueueSize;
        }

        public async Task<RuneQuery> RetrieveRunes(RuneQuery query)
        {
            //todo
            
            return await Task.FromResult(query);
        }
        
        public async Task SendRunes<T>(params T[] runes) where T : Rune
        {
            foreach (var rune in runes)
            {
                //try add
                if (CurrentQueueSize >= MaxQueueSize) RemoveRandomMessage();

                var queue = GetOrCreateQueue(rune.GetType());
                queue.Enqueue(
                    new RuneWrapper()
                    {
                        Rune = rune,
                        CreatedTime = DateTime.Now
                    });
            }
            await Task.CompletedTask;
        }

        public void RemoveRandomMessage()
        {
            var queues = RuneQueues.Where(r => r.Value.Count > 0)
                                   .ToList();
            RuneWrapper result;
            queues[new Random().Next(0, queues.Count - 1)].Value.TryDequeue(out result);
        }

        private ConcurrentQueue<RuneWrapper> GetOrCreateQueue(Type type)
        {
            if (!RuneQueues.ContainsKey(type))
                RuneQueues[type] = new ConcurrentQueue<RuneWrapper>();
            return RuneQueues[type];
        }
    }

    public class RuneWrapper
    {
        public object Rune { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
