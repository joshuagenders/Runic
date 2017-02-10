using System;

namespace Runic.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MinesRunesAttribute : Attribute
    {
        private string[] _runes { get; set; }

        public MinesRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }
    }
}
