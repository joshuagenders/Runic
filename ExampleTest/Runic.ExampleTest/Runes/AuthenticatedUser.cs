using Runic.Framework.Attributes;
using Runic.Framework.Models;

namespace Runic.ExampleTest.Runes
{
    public class AuthenticatedUser : Rune
    {
        public AuthenticatedUser() : base("AuthenticatedUser")
        {
        }

        [IndexedProperty]
        public string Username { get; set; }

        public string Password { get; set; }
    }
}