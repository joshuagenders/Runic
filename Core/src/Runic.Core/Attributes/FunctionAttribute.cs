using System;

namespace Runic.Core.Attributes
{
    public class FunctionAttribute : Attribute
    {
        public string Name { get; set; }

        public FunctionAttribute(string name)
        {
            Name = name;
        }
    }
}
