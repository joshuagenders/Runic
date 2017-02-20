using System;

namespace Runic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresLinkedRunesAttribute : Attribute
    {
        public RequiresLinkedRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }

        private string[] _runes;

        public string[] Runes
        {
            get { return _runes; }
            set { _runes = value; }
        }
    }
}