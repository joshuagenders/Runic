﻿using Runic.Core.Attributes;
using Runic.Core.Models;

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