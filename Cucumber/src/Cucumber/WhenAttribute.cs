using System;

namespace Runic.Cucumber
{
    public class WhenAttribute : Attribute, IRegexMatchable
    {
        public WhenAttribute(string matchString)
        {
            MatchString = matchString;
        }
        public string MatchString { get; set; }
        public string GetMatchString => MatchString;
    }
}