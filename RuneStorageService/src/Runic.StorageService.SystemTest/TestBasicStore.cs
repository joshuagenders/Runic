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
            TestContext = new TestDatabaseContext();
            Database = new ODatabase(TestConnection.ConnectionOptions);

            Database
                .Create.Class<ExampleRune>()
                .Run();
        }

        public void Dispose()
        {
            Database.Close();
            TestConnection.DropTestDatabase();
            TestContext.Dispose();
        }

        private TestDatabaseContext TestContext { get; }
        private ODatabase Database { get; }

        [Fact]
        public void TestStoreAndSqlRetrieve()
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
            var documents = Database
                .Select()
                .From<ExampleRune>()
                .ToList();

            Assert.Equal(documents.Count, 1);
            Assert.Equal(documents[0].GetField<string>("ExampleValue1"), testRunes[0].ExampleValue1);
            Assert.Equal(documents[0].GetField<string>("ExampleValue2"), testRunes[0].ExampleValue2);
        }
    }
}