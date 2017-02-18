using Runic.Core.Attributes;
using System.Reflection;
using Xunit;

namespace Runic.UnitTest.Runic
{
    public class TestAttributes
    {
        [RequiresRunes("123")]
        [Fact]
        public void TestRequiresRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresRuneAttributeData");
            RequiresRunesAttribute attr = method.GetCustomAttribute<RequiresRunesAttribute>();
            Assert.Equal("123", attr.Runes[0]);
        }

        [MutableParameter("myInput", typeof(string))]
        [Fact]
        public void TestMutableParameterAttributeData()
        {
            var method = GetType().GetMethod("TestMutableParameterAttributeData");
            MutableParameterAttribute attr = method.GetCustomAttribute<MutableParameterAttribute>();
            Assert.Equal("myInput", attr.ParameterName);
            Assert.Equal(typeof(string), attr.ParameterType);
        }

        [RequiresLinkedRunes("Login", "Search")]
        [Fact]
        public void TestRequiresLinkedRuneAttributeData()
        {
            var method = GetType().GetMethod("TestRequiresLinkedRuneAttributeData");
            RequiresLinkedRunesAttribute attr = method.GetCustomAttribute<RequiresLinkedRunesAttribute>();
            Assert.Contains(attr.Runes, a => a == "Login");
            Assert.Contains(attr.Runes, a => a == "Search");
        }

        [MinesRunes("123","456")]
        [Fact]
        public void TestMinesRuneAttributeData()
        {
            var method = GetType().GetMethod("TestMinesRuneAttributeData");
            MinesRunesAttribute attr = method.GetCustomAttribute<MinesRunesAttribute>();
            Assert.Equal("123", attr.Runes[0]);
            Assert.Equal("456", attr.Runes[1]);
        }
    }
}
