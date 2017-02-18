using System;

namespace Runic.RuneStorageService.SystemTest
{
    public class TestDatabaseContext : IDisposable
    {
        public TestDatabaseContext()
        {
            TestConnection.CreateTestDatabase();
        }

        public void Dispose()
        {
            TestConnection.DropTestDatabase();
        }
    }
}
