using System;

namespace Runic.Agent.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresRunesAttribute : Attribute
    {
        public RequiresRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }

        private string[] _runes;

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