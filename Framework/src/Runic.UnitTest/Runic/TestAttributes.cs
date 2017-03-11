using System.Reflection;
using Runic.Framework.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Runic.UnitTest.Runic
{
    [TestClass]
    public class TestAttributes
    {
        [MinesRunes("123", "456")]
        [TestMethod]
        public void TestMinesRuneAttributeData()
        {
            var method = GetType().GetMethod("TestMinesRuneAttributeData");
            var attr = method.GetCustomAttribute<MinesRunesAttribute>();
            Assert.AreEqual("123", attr.Runes[0]);
            Assert.AreEqual("456", attr.Runes[1]);
        }

        [MutableParameter("myInput", typeof(string))]
        [TestMethod]
        public void TestMutableParameterAttributeData()
        {
            var method = GetType().GetMethod("TestMutableParameterAttributeData");
            var attr = method.GetCustomAttribute<MutableParameterAttribute>();
            Assert.AreEqual("myInput", attr.ParameterName);
            Assert.AreEqual(typeof(string), attr.ParameterType);
        }

        [RequiresLinkedRunes("Login", "Search")]
        [TestMethod]
        public void TestRequiresLinkedRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresLinkedRuneAttributeData");
            var attr = method.GetCustomAttribute<RequiresLinkedRunesAttribute>();
            Assert.IsTrue(attr.Runes.ToList().Contains("Login"));
            Assert.IsTrue(attr.Runes.ToList().Contains("Search"));
        }

        [RequiresRunes("123")]
        [TestMethod]
        public void TestRequiresRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresRuneAttributeData");
            var attr = method.GetCustomAttribute<RequiresRunesAttribute>();
            Assert.AreEqual("123", attr.Runes[0]);
        }
    }
}