using System;

namespace Runic.Cucumber
{
    public class GivenAttribute : Attribute, IRegexMatchable
    {
        public GivenAttribute(string matchString)
        {
            MatchString = matchString;
        }
        public string MatchString { get; set; }
        public string GetMatchString => MatchString;
    }
}
//combine this with runic, part of the framework
//shared datatable inputs from an interface provider, queues, files etc
//execute as steps
//find a way to tie runes queries into the language
//use string input 
// given I have a rune x (where x is a string that queries a dictionary of RuneQueries
//a way to tie datasources into the language

//two fluent interfaces, one for exeuting a whole document
//eg. CucumberTest.Execute(some string..)
//one for executing a document in code
//eg.
//Test.Given("Something "datasource:somesource.somefield")
//    .When(...
//    .Execute().ContinueOnError();
//introduce concept of a private rune? marked so only one thread can use it, guid as key or something?