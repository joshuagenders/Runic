using System;
using System.Collections.Generic;
using Orient.Client;
using Xunit;

namespace Runic.RuneStorageService.SystemTest
{
    public class TestBasicStore : IDisposable
    {
        public TestBasicStore()
        {
            _testContext = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.ConnectionOptions);

            _database
                .Create.Class<ExampleRune>()
                .Run();
        }

        public void Dispose()
        {
            _database.Close();
            TestConnection.DropTestDatabase();
            _testContext.Dispose();
        }

        private TestDatabaseContext _testContext { get; }
        private ODatabase _database { get; }

        [Fact]
        public void TestStoreAndSQLRetrieve()
        {
            var testRunes = new List<ExampleRune>
            {
                new ExampleRune
                {
                    ExampleValue1 = Guid.NewGuid().ToString("n"),
                    ExampleValue2 = Guid.NewGuid().ToString("n")
                }
            };
            var service = new RuneStorageService();
            service.StoreRunes(testRunes);
            var documents = _database
                .Select()
                .From<ExampleRune>()
                .ToList();

            Assert.Equal(documents.Count, 1);
            Assert.Equal(documents[0].GetField<string>("ExampleValue1"), testRunes[0].ExampleValue1);
            Assert.Equal(documents[0].GetField<string>("ExampleValue2"), testRunes[0].ExampleValue2);
        }
    }
}