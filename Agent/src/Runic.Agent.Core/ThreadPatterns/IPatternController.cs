using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadPatterns
{
    public interface IPatternController
    {
        void StartThreadPattern(string id, Flow flow, IThreadPattern pattern, CancellationToken ctx);
        Task Stop(string id, CancellationToken ctx);
        Task StopAll(CancellationToken ctx);
        Task Run(CancellationToken ctx);
    }
}
