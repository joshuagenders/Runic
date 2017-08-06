using System;

namespace Runic.Cucumber
{
    public abstract class CucumberAttribute : Attribute, IRegexMatchable
    {
        public CucumberAttribute(string matchString)
        {
            MatchString = matchString;
        }
        public string MatchString { get; set; }
        public string GetMatchString => MatchString;
    }

    public class GivenAttribute : CucumberAttribute
    {
        public GivenAttribute(string matchString) : base(matchString)
        {
        }
    }
    public class WhenAttribute : CucumberAttribute
    {
        public WhenAttribute(string matchString) : base(matchString)
        {
        }
    }
    public class ThenAttribute : CucumberAttribute
    {
        public ThenAttribute(string matchString) : base(matchString)
        {
        }
    }
    public class AndAttribute : CucumberAttribute
    {
        public AndAttribute(string matchString) : base(matchString)
        {
        }
    }
}