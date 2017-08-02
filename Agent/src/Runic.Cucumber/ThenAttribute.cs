using System;

namespace Runic.Cucumber
{
    public class ThenAttribute : Attribute, IRegexMatchable
    {
        public ThenAttribute(string matchString)
        {
            MatchString = matchString;
        }
        public string MatchString { get; set; }
        public string GetMatchString => MatchString;
    }
}