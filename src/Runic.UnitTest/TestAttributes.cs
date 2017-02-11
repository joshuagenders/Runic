using Runic.Attributes;
using System.Reflection;
using Xunit;

namespace Runic.UnitTest
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
