using System;

namespace Runic.Agent.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {
        public FunctionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}