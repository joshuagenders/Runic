using System;

namespace Runic.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MinesRunesAttribute : Attribute
    {
        private string[] _runes { get; set; }
        public string[] Runes { get { return _runes; } set { _runes = value; } }

        public MinesRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }
    }
}
