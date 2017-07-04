using System;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace Runic.Agent.Core.Data
{
    public class MemoryDataStore : IDataStore
    {
        private ConcurrentDictionary<Type, ConcurrentBag<object>> _runeQueue;
        public MemoryDataStore()
        {
            _runeQueue = new ConcurrentDictionary<Type, ConcurrentBag<object>>();
        }

        public T GetRune<T>() where T : Rune
        {
            if (!_runeQueue.ContainsKey(typeof(T)) || _runeQueue[typeof(T)] == null)
                return null;
            object rune;
            return _runeQueue[typeof(T)].TryTake(out rune) ? (T)rune : null;
        }

        public T GetRune<T>(RuneQuery query) where T : Rune
        {
            //todo 
            //throw new NotImplementedException();
            //var bag = GetCreateRuneBag<T>() as dynamic;
            //bag.Where(r => r.GetType())
        }

        private ConcurrentBag<object> GetCreateRuneBag<T>() where T : Rune
        {
            if (!_runeQueue.ContainsKey(typeof(T)) || _runeQueue[typeof(T)] == null)
            {
                _runeQueue[typeof(T)] = new ConcurrentBag<T>() as ConcurrentBag<object>;
            }
            return _runeQueue[typeof(T)];
        }

        public void StoreRune<T>(T rune) where T : Rune
        {
            //todo ttl
            GetCreateRuneBag<T>().Add(rune);
        }

        public T UseRune<T>(T rune) where T : Rune
        {
            throw new NotImplementedException();
        }
    }
}
