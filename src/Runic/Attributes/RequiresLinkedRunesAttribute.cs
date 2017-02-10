using System;

namespace Runic.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresLinkedRunesAttribute : Attribute
    {
        private string _propertyName { get; set; }
        private string[] _runes { get; set; }
        public RequiresLinkedRunesAttribute(string propertyName, params string[] runes)
        {
            _runes = runes;
            _propertyName = propertyName;
        }
    }
}
