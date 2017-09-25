using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public class TaskQueue<T> : IList<T>
    {
        private ConcurrentDictionary<string, T> Items { get; set; }
        public TaskQueue()
        {
            Items = new ConcurrentDictionary<string, T>();
        }
        
        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            Items.TryAdd(Guid.NewGuid().ToString("N"), item);
        }

        public void Clear()
        {
            Items = new ConcurrentDictionary<string, T>();
        }

        private bool CycleCheckCondition(Func<T, bool> func)
        {
            lock (Items)
            {
                bool found = false;
                Parallel.ForEach(Items.Keys, (key) => 
                {
                    if (func(Items[key])) found = true;
                });
                return found;
            }
        }

        public bool Contains(T item)
        {
            return CycleCheckCondition(t => t.Equals(item));
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
