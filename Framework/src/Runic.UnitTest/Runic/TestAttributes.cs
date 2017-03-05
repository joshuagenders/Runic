using System.Reflection;
using Runic.Core.Attributes;
using Xunit;

namespace Runic.UnitTest.Runic
{
    public class TestAttributes
    {
        [MinesRunes("123", "456")]
        [Fact]
        public void TestMinesRuneAttributeData()
        {
            var method = GetType().GetMethod("TestMinesRuneAttributeData");
            var attr = method.GetCustomAttribute<MinesRunesAttribute>();
            Assert.Equal("123", attr.Runes[0]);
            Assert.Equal("456", attr.Runes[1]);
        }

        [MutableParameter("myInput", typeof(string))]
        [Fact]
        public void TestMutableParameterAttributeData()
        {
            var method = GetType().GetMethod("TestMutableParameterAttributeData");
            var attr = method.GetCustomAttribute<MutableParameterAttribute>();
            Assert.Equal("myInput", attr.ParameterName);
            Assert.Equal(typeof(string), attr.ParameterType);
        }

        [RequiresLinkedRunes("Login", "Search")]
        [Fact]
        public void TestRequiresLinkedRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresLinkedRuneAttributeData");
            var attr = method.GetCustomAttribute<RequiresLinkedRunesAttribute>();
            Assert.Contains(attr.Runes, a => a == "Login");
            Assert.Contains(attr.Runes, a => a == "Search");
        }

        [RequiresRunes("123")]
        [Fact]
        public void TestRequiresRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresRuneAttributeData");
            var attr = method.GetCustomAttribute<RequiresRunesAttribute>();
            Assert.Equal("123", attr.Runes[0]);
        }
    }
}