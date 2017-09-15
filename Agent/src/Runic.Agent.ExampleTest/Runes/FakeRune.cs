using Runic.Agent.Framework.Models;

namespace Runic.Agent.TestUtility.Runes
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
