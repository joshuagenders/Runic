﻿using Runic.Core.Models;

namespace Runic.ExampleTest.Runes
{
    public class BasketCreated : Rune
    {
        public BasketCreated() : base("BasketCreated")
        {
        }

        public string BasketId { get; set; }
    }
}