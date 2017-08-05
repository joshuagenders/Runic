using Runic.Framework.Models;

namespace Runic.Agent.Standalone.Test.TestUtility
{
    public class FakeRune : Rune
    {
        public FakeRune()
        {
            Name = "FakeRune";
        }
        public string Data { get; set; }
    }
}
