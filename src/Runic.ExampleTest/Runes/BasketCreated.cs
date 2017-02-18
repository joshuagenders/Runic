using Runic.Core;

namespace Runic.SystemTest.Runes
{
    public class BasketCreated : Rune
    {
        public string BasketId { get; set; }
        public BasketCreated () : base("BasketCreated") { }
    }
}
