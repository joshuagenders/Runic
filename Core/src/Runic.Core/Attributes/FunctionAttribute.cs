using System;

namespace Runic.Core.Attributes
{
    public class FunctionAttribute : Attribute
    {
        public FunctionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}