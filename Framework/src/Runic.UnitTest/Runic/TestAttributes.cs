using System.Reflection;
using Runic.Core.Attributes;
using NUnit.Framework;

namespace Runic.UnitTest.Runic
{
    public class TestAttributes
    {
        [MinesRunes("123", "456")]
        [Test]
        public void TestMinesRuneAttributeData()
        {
            var method = GetType().GetMethod("TestMinesRuneAttributeData");
            var attr = method.GetCustomAttribute<MinesRunesAttribute>();
            Assert.AreEqual("123", attr.Runes[0]);
            Assert.AreEqual("456", attr.Runes[1]);
        }

        [MutableParameter("myInput", typeof(string))]
        [Test]
        public void TestMutableParameterAttributeData()
        {
            var method = GetType().GetMethod("TestMutableParameterAttributeData");
            var attr = method.GetCustomAttribute<MutableParameterAttribute>();
            Assert.AreEqual("myInput", attr.ParameterName);
            Assert.AreEqual(typeof(string), attr.ParameterType);
        }

        [RequiresLinkedRunes("Login", "Search")]
        [Test]
        public void TestRequiresLinkedRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresLinkedRuneAttributeData");
            var attr = method.GetCustomAttribute<RequiresLinkedRunesAttribute>();
            Assert.Contains("Login", attr.Runes);
            Assert.Contains("Search", attr.Runes);
        }

        [RequiresRunes("123")]
        [Test]
        public void TestRequiresRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresRuneAttributeData");
            var attr = method.GetCustomAttribute<RequiresRunesAttribute>();
            Assert.AreEqual("123", attr.Runes[0]);
        }
    }
}