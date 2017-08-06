using System;

namespace Runic.Cucumber
{
    public class GherkinDocumentParserError : Exception
    {
        public GherkinDocumentParserError()
        {
        }

        public GherkinDocumentParserError(string message) : base(message)
        {
        }

        public GherkinDocumentParserError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}