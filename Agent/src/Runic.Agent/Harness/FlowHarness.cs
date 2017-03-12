﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using StatsN;

namespace Runic.Agent.Harness
{
    public class FlowHarness : IFlowHarness
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IStatsd _statsd { get; }

        private Flow _flow { get; set; }
        private int _threadCount { get; set; }

        private SemaphoreSlim _semaphore { get; set; }
        private ConcurrentBag<Task> _trackedTasks { get; set; }
        private Dictionary<string, object> _instances { get; set; }
        private List<CancellationTokenSource> _cancellationSources { get; set; }

        public void InstantiateCollections()
        {
            _instances = new Dictionary<string, object>();
            _cancellationSources = new List<CancellationTokenSource>();
            _trackedTasks = new ConcurrentBag<Task>();
        }

        public FlowHarness()
        {
            IStatsd statsd;
            if (IoC.Container.TryResolve<IStatsd>(out statsd))
                _statsd = statsd;
            InstantiateCollections();
        }

        public async Task Execute(Flow flow, int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Debug($"Executing flow {flow.Name}");
            _statsd.Count($"flows.{flow.Name}.flowStarted");
            InstantiateCollections();

            _flow = flow;
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount, _threadCount);

            while (!ctx.IsCancellationRequested && threadCount > 0)
            {
                //get thread permission
                await _semaphore.WaitAsync(ctx);

                _logger.Debug($"Starting thread for {flow.Name}");
                _statsd.Count($"flows.{flow.Name}.threadStarted");
                var cts = new CancellationTokenSource();
                _cancellationSources.Add(cts);
                _trackedTasks.Add(ExecuteFlow(cts.Token).ContinueWith((_) =>
                {
                    if (_.Exception != null)
                        _logger.Error(_.Exception);

                    cts.Cancel();
                    _cancellationSources.Remove(cts);
                    //free thread
                    _semaphore.Release();
                }, ctx));
            }
            _cancellationSources.ForEach(c => c.Cancel());
            await Task.WhenAll(_trackedTasks);
            _logger.Debug($"Completed flow execution for {flow.Name}");
            _statsd?.Count($"flows.{flow.Name}.flowCompleted");
        }

        public List<Task> GetTasks ()
        {
            return _trackedTasks.ToList();
        }

        public int GetTotalInitiatiedThreadCount()
        {
            return _trackedTasks.Count();
        }

        public int GetRunningThreadCount()
        {
            var completedStatuses = new[]
            {
                TaskStatus.RanToCompletion,
                TaskStatus.Canceled,
                TaskStatus.Faulted
            };
            return _trackedTasks.Count(t => !completedStatuses.Contains(t.Status));
        }

        public int GetSemaphoreCurrentCount()
        {
            return _semaphore.CurrentCount;
        }

        public Flow GetRunningFlow()
        {
            return _flow;
        }

        private async Task ExecuteFlow(CancellationToken ctx = default(CancellationToken))
        {
            foreach (var step in _flow.Steps)
            {
                try
                {
                    //load the library if needed
                    LoadLibrary(step);
                    //instantiate the class if needed
                    InitialiseFunction(step);
                }
                catch (Exception e)
                {
                    _logger.Error($"Encountered error initialising flow {_flow.Name}");
                    _logger.Error(e);
                }
                
            }

            while (!ctx.IsCancellationRequested)
            {
                //todo handle complex flows
                foreach (var step in _flow.Steps)
                {
                    //execute with function harness
                    var instance = _instances[step.Function.FunctionName];
                    var functionHarness = IoC.Container.Resolve<IFunctionHarness>();
                    functionHarness.Bind(instance);

                    Thread.Sleep(_flow.StepDelayMilliseconds);
                    _logger.Debug($"Executing step for {_flow.Name}");
                    try
                    {
                        await functionHarness.Execute(step.Function.FunctionName, ctx);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                    }
                }
            }
        }

        private void RestartThreads(int threadCount)
        {
            CancelAllThreads();
            _threadCount = threadCount;
            _semaphore = new SemaphoreSlim(_threadCount);
        }

        public async Task UpdateThreads(int threadCount, CancellationToken ctx = default(CancellationToken))
        {
            _logger.Debug($"Updating threads for {_flow.Name} from {GetRunningThreadCount()} to {threadCount}");
            //todo if < > ==
            await Task.Run(() => RestartThreads(threadCount), ctx);
        }

        private void LoadLibrary(Step step)
        {
            _logger.Debug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
            PluginManager.LoadPlugin(step.Function.AssemblyName);
        }

        private void InitialiseFunction(Step step)
        {
            _logger.Debug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            if (_instances.ContainsKey(step.Function.FunctionName))
                return;

            _logger.Debug($"Retrieving function type");

            var type = PluginManager.GetFunctionType(step.Function.FunctionName);
            if (type == null)
                throw new FunctionTypeNotFoundException();

            _logger.Debug($"type found {type.AssemblyQualifiedName}");

            _instances[step.Function.FunctionName] = Activator.CreateInstance(type);
            _logger.Debug($"{step.Function.FunctionName} in {step.Function.AssemblyName} initialised");
        }

        private void CancelAllThreads()
        {
            _cancellationSources.ForEach(t => t.Cancel());   
        }
    }

}
