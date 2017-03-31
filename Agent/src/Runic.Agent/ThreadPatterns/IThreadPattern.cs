﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.ThreadPatterns
{
    public interface IThreadPattern
    {
        void RegisterThreadChangeHandler(Action<int> callback);
        Task Start(CancellationToken ct);
        int GetMaxDurationSeconds();
        int GetMaxThreadCount();
    }
}
