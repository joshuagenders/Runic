using System;

namespace Runic.Agent.Standalone.Services
{
    internal class ThreadPatternNotRecognisedException : Exception
    {
        public ThreadPatternNotRecognisedException()
        {
        }

        public ThreadPatternNotRecognisedException(string message) : base(message)
        {
        }

        public ThreadPatternNotRecognisedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}