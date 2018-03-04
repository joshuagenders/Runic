using System;
using System.IO;

namespace ExampleTest
{
    public class ExampleClass
    {
        public void ExampleMethod()
        {
            var filename = $"{Guid.NewGuid().ToString("N")}.testgeneratedfile.txt";
            using (var fh = File.CreateText(filename))
            {
                fh.Write("FooBar");
            }
        }
    }
}
