using System;

namespace Runic.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MutableParameterAttribute : Attribute
    {
        private string _parameterName { get; set; }
        private Type _parameterType { get; set; }

        public string ParameterName { get { return _parameterName; } set { _parameterName = value; } }
        public Type ParameterType { get { return _parameterType; } set { _parameterType = value; } }

        public MutableParameterAttribute(string parameterName, Type parameterType)
        {
            _parameterName = parameterName;
            _parameterType = parameterType;
        }
    }
}
