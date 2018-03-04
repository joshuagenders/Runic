using Runic.Cucumber;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExampleTest
{
    public class ExampleClass
    {
        private List<string> _cucumberState { get; set; } = new List<string>();

        public void ExampleMethod()
        {
            var filename = $"{Guid.NewGuid().ToString("N")}.testgeneratedfile.txt";
            using (var fh = File.CreateText(filename))
            {
                fh.Write("FooBar");
            }
        }

        public async Task ExampleMethodAsync()
        {
            var filename = $"{Guid.NewGuid().ToString("N")}.testgeneratedfile.txt";
            await File.WriteAllTextAsync(filename, "FooBar"); 
        }

        public void ExampleMethodWithParams(string stringInput, int intInput)
        {
            var filename = $"{Guid.NewGuid().ToString("N")}.testgeneratedfile.txt";
            using (var fh = File.CreateText(filename))
            {
                fh.Write($"StringInput: {stringInput}, IntInput: {intInput}");
            }
        }

        [Given("I have set up preconditions")]
        public void Given()
        {
            _cucumberState.Add("Given");
        }

        [When("I execute some action")]
        public void When()
        {
            _cucumberState.Add("When");
        }

        [Then("I observe some result")]
        public void Then()
        {
            if (!_cucumberState.Contains("Given"))
            {
                throw new Exception("Given state not found");
            }
            if (!_cucumberState.Contains("When"))
            {
                throw new Exception("When state not found");
            }
            if (_cucumberState.Count != 2)
            {
                throw new Exception($"State count was meant to be 2 and was {_cucumberState.Count}");
            }
        }
    }
}
