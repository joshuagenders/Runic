using System;

namespace Runic.Framework.Attributes
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