using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Orient.Client;

namespace Runic.RuneStorageService.SystemTest
{
    public class TestBasicStore : IDisposable
    {
        TestDatabaseContext _testContext { get; set; }
        ODatabase _database { get; set; }

        public TestBasicStore()
        {
            _testContext = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.ConnectionOptions);
            
            _database
                .Create.Class<ExampleRune>()
                .Run();
        }

        [Fact]
        public void TestStoreAndSQLRetrieve()
        {
            var testRunes = new List<ExampleRune>() {
                new ExampleRune()
                {
                    ExampleValue1 = Guid.NewGuid().ToString("n"),
                    ExampleValue2 = Guid.NewGuid().ToString("n")
                }
            };
            var service = new RuneStorageService();
            service.StoreRunes(testRunes);
            List<ODocument> documents = _database
                        .Select()
                        .From<ExampleRune>()
                        .ToList();

            Assert.Equal(documents.Count, 1);
            Assert.Equal(documents[0].GetField<string>("ExampleValue1"),testRunes[0].ExampleValue1);
            Assert.Equal(documents[0].GetField<string>("ExampleValue2"), testRunes[0].ExampleValue2);
        }

        public void Dispose()
        {
            _database.Close();
            TestConnection.DropTestDatabase();
            _testContext.Dispose();
        }
    }
}
