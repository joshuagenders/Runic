using System;

namespace Runic.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresLinkedRunesAttribute : Attribute
    {
        private string[] _runes { get; set; }

        public string[] Runes { get { return _runes; } set { _runes = value; } }

        public RequiresLinkedRunesAttribute(params string[] runes)
        {
            _runes = runes;
        }
    }
}
