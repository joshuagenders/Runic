using System;

namespace Runic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresRunesAttribute : Attribute
    {
        private string[] _runes { get; set; }
        public string[] Runes { get { return _runes; } set { _runes = value; } }

        public RequiresRunesAttribute (params string[] runes)
        {
            _runes = runes;
        }
    }
}
