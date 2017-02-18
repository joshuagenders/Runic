using Runic.Core.Messaging;

namespace Runic.SystemTest.Runes
{
    public class BasketCreated : Rune
    {
        public string BasketId { get; set; }
        public BasketCreated () : base("BasketCreated") { }
    }
}
