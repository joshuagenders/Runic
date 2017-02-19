using Moq;
using Runic.Clients;
using Runic.Data;
using Runic.Core.Models;
using System.Threading.Tasks;
using Xunit;

namespace Runic.UnitTest.Runic
{
    public class TestDbClient
    {
        [Fact]
        public void TestMockedMine()
        {
            var testRune = new ExampleRune();
            Mock<IRuneClient> clientMock = new Mock<IRuneClient>();
            clientMock.Setup(x => x.SendRunes(testRune));
       
            Runes.Client = clientMock.Object;
            
            Runes.Mine(testRune);

            clientMock.Verify(c => c.SendRunes(testRune));
        }

        [Fact]
        public async void TestMockedRetrieve()
        {
            var runeQuery = new RuneQuery()
            {
                RuneName = "ExampleRune"
            };

            Mock<IRuneClient> clientMock = new Mock<IRuneClient>();
            clientMock.Setup(x => x.RetrieveRunes(runeQuery))
                      .Returns<RuneQuery>((o) =>
                      {
                          return Task.Run(() =>
                          {
                              o.Result = new ExampleRune();
                              return o;
                          });
                      });

            Runes.Client = clientMock.Object;

            var rune = await Runes.Retrieve(runeQuery);
            
            Assert.Equal(rune.RuneName, "ExampleRune");
        }
    }
    
    public class ExampleRune : Rune
    {
        public ExampleRune() : base("ExampleRune") { }
    }
}
