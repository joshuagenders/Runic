using System;

namespace Runic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MinesRunesAttribute : Attribute
    {
        private string[] _runes;

        public MinesRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }

        public string[] Runes
        {
            get { return _runes; }
            set { _runes = value; }
        }
    }
}