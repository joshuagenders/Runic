using System;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Runic.Agent.Core.Data
{
    public class MemoryDataStore : IDataStore
    {
        private ConcurrentDictionary<Type, ConcurrentQueue<object>> _runeQueue;
        public MemoryDataStore()
        {
            _runeQueue = new ConcurrentDictionary<Type, ConcurrentQueue<object>>();
        }

        public T GetRune<T>() where T : Rune
        {
            if (!_runeQueue.ContainsKey(typeof(T)) || _runeQueue[typeof(T)] == null)
                return null;
            object rune;
            return _runeQueue[typeof(T)].TryDequeue(out rune) ? (T)rune : null;
        }

        public T GetRune<T>(RuneQuery query) where T : Rune
        {
            throw new NotImplementedException();
        }

        public void StoreRune<T>(T rune) where T : Rune
        {
            //todo ttl
            if (!_runeQueue.ContainsKey(typeof(T)))
            {
                _runeQueue[typeof(T)] = new ConcurrentQueue<object>();
            }
            else if (_runeQueue[typeof(T)] == null)
            {
                _runeQueue[typeof(T)] = new ConcurrentQueue<object>();
            }
            
            _runeQueue[typeof(T)].Enqueue(rune);
        }

        public T UseRune<T>(T rune) where T : Rune
        {
            throw new NotImplementedException();
        }
    }
}
