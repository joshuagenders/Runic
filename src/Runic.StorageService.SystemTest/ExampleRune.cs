using Runic.Core.Messaging;

namespace Runic.RuneStorageService.SystemTest
{
    public class ExampleRune : Rune
    {
        public ExampleRune() : base("ExampleRune") { }
        public string ExampleValue1 { get; set; }
        public string ExampleValue2 { get; set; }
    }
}
