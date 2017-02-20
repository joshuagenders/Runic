using System;

namespace Runic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MutableParameterAttribute : Attribute
    {
        public MutableParameterAttribute(string parameterName, Type parameterType)
        {
            _parameterName = parameterName;
            _parameterType = parameterType;
        }

        private string _parameterName;
        private Type _parameterType;

        public string ParameterName
        {
            get { return _parameterName; }
            set { _parameterName = value; }
        }

        public Type ParameterType
        {
            get { return _parameterType; }
            set { _parameterType = value; }
        }
    }
}