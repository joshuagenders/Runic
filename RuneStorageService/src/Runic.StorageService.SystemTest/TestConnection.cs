using Orient.Client;
using OrientDB_Net.binary.Innov8tive.API;

namespace Runic.RuneStorageService.SystemTest
{
    public static class TestConnection
    {
        private static readonly string _hostname = "localhost";
        private static readonly int _port = 2424;
        private static readonly string _username = "root";
        private static readonly string _password = "root";

        private static readonly string _rootUserName = "root";
        private static readonly string _rootUserParssword = "root";
        private static readonly OServer _server;

        static TestConnection()
        {
            _server = new OServer(_hostname, _port, _rootUserName, _rootUserParssword);

            GlobalTestDatabaseName = "RunicTest";
            GlobalTestDatabaseType = ODatabaseType.Document;
            GlobalTestDatabaseAlias = "globalTestDb";
        }

        public static int GlobalTestDatabasePoolSize
        {
            get { return 3; }
        }

        public static string GlobalTestDatabaseName { get; }
        public static ODatabaseType GlobalTestDatabaseType { get; }
        public static string GlobalTestDatabaseAlias { get; private set; }

        public static ConnectionOptions ConnectionOptions
        {
            get
            {
                return new ConnectionOptions
                {
                    DatabaseType = GlobalTestDatabaseType,
                    Port = _port,
                    Password = _password,
                    HostName = _hostname,
                    DatabaseName = GlobalTestDatabaseName,
                    UserName = _username
                };
            }
        }

        public static void CreateTestDatabase()
        {
            DropTestDatabase();

            _server.CreateDatabase(GlobalTestDatabaseName, GlobalTestDatabaseType, OStorageType.PLocal);
        }

        public static void DropTestDatabase()
        {
            if (_server.DatabaseExist(GlobalTestDatabaseName, OStorageType.PLocal))
                _server.DropDatabase(GlobalTestDatabaseName, OStorageType.PLocal);
        }

        public static OServer GetServer()
        {
            return _server;
        }
    }
}