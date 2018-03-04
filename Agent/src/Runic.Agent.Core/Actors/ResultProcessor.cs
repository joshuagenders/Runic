using Akka.Actor;
using Akka.Event;
using Runic.Agent.Core.Models;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Core.Actors
{
    public class ResultProcessor : ReceiveActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        public ResultProcessor()
        {
            Receive<List<Result>>(_ => HandleTestResults(_));
        }

        private void HandleTestResults(List<Result> results)
        {
            foreach (var result in results)
            {
                if (result.Success)
                {
                    Log.Info("Journey success");
                }
                else
                {
                    Log.Error($"Journey Failure: {result.ExceptionMessage}");
                }
            }
        }
    }
}
