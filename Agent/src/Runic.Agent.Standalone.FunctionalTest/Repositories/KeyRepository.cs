using System;
using System.Collections.Generic;

namespace Runic.Agent.FunctionalTest.Repositories
{
    public abstract class KeyRepository<T>
    {
        protected Dictionary<string, T> _values { get; set; } = new Dictionary<string, T>();
        public T Get(string key)
        {
            if (!_values.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException($"There was no value in the {nameof(T)} repository for key {key}");
            }
            return _values[key];
        }
    }
}
