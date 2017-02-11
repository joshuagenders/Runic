using Moq;
using Newtonsoft.Json;
using Runic.Clients;
using Runic.Data;
using Runic.Query;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Runic.UnitTest
{
    public class TestDbClient
    {
        [Fact]
        public void TestMockedMine()
        {
            Mock<IRuneClient> clientMock = new Mock<IRuneClient>();
            clientMock.Setup(x => x.SendRunes(null))
                      .Returns(new Task<HttpResponseMessage>(() => 
                      {
                          return new HttpResponseMessage()
                          {
                              StatusCode = System.Net.HttpStatusCode.Created
                          };
                      }));
       
            Runes.Client = clientMock.Object;

            // Act
            Runes.Mine(new Rune()
            {
                Name = "TestName1"
            });
        }

        [Fact]
        public async void TestMockedRetrieve()
        {
            Mock<IRuneClient> clientMock = new Mock<IRuneClient>();
            clientMock.Setup(x => x.RetrieveRunes())
                      .Returns(Task.Run(() => {
                          return new HttpResponseMessage()
                          {
                              StatusCode = System.Net.HttpStatusCode.OK,
                              Content = new StringContent("['satu', 'dua', ''tiga']")
                          };
                      }));

            Runes.Client = clientMock.Object;

            // Act
            var rune = await Runes.Retrieve(new RuneQuery()
            {
                RuneName = "blah",
                EnableRegex = true,
                LinkedProperties = new string[] { "1","2","3"},
                RequiredProperties = new Dictionary<string, string>()
                {
                    { "1", "2" }
                }
            });

            var result = JsonConvert.DeserializeObject<List<string>>(rune.Detail.ToString());

            Assert.Equal(result[0], "satu");
            Assert.Equal(result[2], "dua");
            Assert.Equal(result[3], "tiga");
        }
    }
}
