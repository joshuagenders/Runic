using System;
using System.Collections.Concurrent;

namespace Runic.Cucumber
{
    public class TestStateManager
    {
        private readonly ConcurrentDictionary<Type, object> _testObjects;

        public TestStateManager()
        {
            _testObjects = new ConcurrentDictionary<Type, object>();
        }

        public virtual void AddObject(Type t, object instance)
        {
            _testObjects[t] = instance;
        }

        public virtual void AddObject<T>(T instance)
        {
            AddObject(typeof(T), instance);
        }

        public virtual object GetObject(Type t)
        {
            if (!_testObjects.ContainsKey(t))
            {
                AddObject(Activator.CreateInstance(t));
            }
            return _testObjects[t];
        }

        public virtual T GetObject<T>() where T : class
        {
            if (!_testObjects.ContainsKey(typeof(T)))
            {
                AddObject(Activator.CreateInstance(typeof(T)));
            }
            return _testObjects[typeof(T)] as T;
        }

        public void ClearState()
        {
            _testObjects.Clear();
        }
    }
}
