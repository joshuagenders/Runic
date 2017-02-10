using System;

namespace Runic.Attributes
{
    public class MutableParameterAttribute : Attribute
    {
        private string _parameterName { get; set; }
        public MutableParameterAttribute(string parameterName, Type parameterType)
        {
            _parameterName = parameterName;
        }
    }
}
