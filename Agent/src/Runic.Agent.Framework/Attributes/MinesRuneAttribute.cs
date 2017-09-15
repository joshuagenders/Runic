using System;

namespace Runic.Agent.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MinesRunesAttribute : Attribute
    {
        private string[] _runes;

        public MinesRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }

        public string[] GetRunes()
        {
            return _runes;
        }

        public void SetRunes(string[] value)
        {
            _runes = value;
        }
    }
}