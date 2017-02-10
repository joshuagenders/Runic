using System;

namespace Runic.Attributes
{
    public class RequiresRunesAttribute : Attribute
    {
        private string[] _runes { get; set; }

        public RequiresRunesAttribute (params string[] runes)
        {
            _runes = runes;
        }
    }
}
