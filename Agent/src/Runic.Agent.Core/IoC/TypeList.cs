using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Core.ThreadManagement;
using System.Collections.Generic;
using System;

namespace Runic.Agent.Core.IoC
{
    public static class TypeList
    {
        public static List<Tuple<Type,Type>> GetDefaultTypeList()
        {
            return new List<Tuple<Type, Type>>()
            {
                Tuple.Create(typeof(PluginManager), typeof(IPluginManager)),
                Tuple.Create(typeof(ThreadManager), typeof(IThreadManager)),
                Tuple.Create(typeof(DateTimeService), typeof(IDatetimeService)),
                Tuple.Create(typeof(RunnerService), typeof(IRunnerService)),
                Tuple.Create(typeof(DataService), typeof(IDataService)),
                Tuple.Create(typeof(EventService), typeof(IEventService))
            };
        }
    }
}
