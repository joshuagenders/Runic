using Moq;
using Runic.Clients;
using Runic.Data;
using RunicCore.Query;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Runic.UnitTest.Runic
{
    public class TestDbClient
    {
        [Fact]
        public void TestMockedMine()
        {
            var testRune = new Rune()
            {
                Name = "TestName1"
            };

            Mock<IRuneClient> clientMock = new Mock<IRuneClient>();
            clientMock.Setup(x => x.SendRunes(testRune))
                      .Returns(new Task<HttpResponseMessage>(() => 
                      {
                          return new HttpResponseMessage()
                          {
                              StatusCode = System.Net.HttpStatusCode.Created
                          };
                      }));
       
            Runes.Client = clientMock.Object;

            // Act
            Runes.Mine(testRune);

            clientMock.Verify(c => c.SendRunes(testRune));
        }

        [Fact]
        public async void TestMockedRetrieve()
        {
            var runeQuery = new RuneQuery()
            {
                RuneName = "blah",
                EnableRegex = true,
                LinkedProperties = new string[] { "1", "2", "3" },
                RequiredProperties = new Dictionary<string, string>()
                {
                    { "1", "2" }
                }
            };

            Mock<IRuneClient> clientMock = new Mock<IRuneClient>();
            clientMock.Setup(x => x.RetrieveRunes(runeQuery))
                      .Returns(Task.Run(() => {
                          return new List<Rune>()
                          {
                              new Rune()
                              {
                                  Name = "satu"
                              }
                          };
                      }));

            Runes.Client = clientMock.Object;

            // Act
            var rune = await Runes.Retrieve(runeQuery);
            
            Assert.Equal(rune.Name, "satu");
        }
    }
}
