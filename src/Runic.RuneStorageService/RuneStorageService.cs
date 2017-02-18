using Runic.Core;
using System.Collections.Generic;
using Orient.Client;
using OrientDB_Net.binary.Innov8tive.API;
using System;
using Orient.Client.API.Query;
using System.Linq;

namespace Runic.RuneStorageService
{
    public class RuneStorageService : IDisposable
    {
        private ODatabase _database { get; set; }

        public RuneStorageService()
        {
            _database = new ODatabase(new ConnectionOptions() { });
        }

        public void StoreRunes<T>(List<T> runes) where T : Rune
        {
            foreach (var rune in runes)
            {
                ODocument document = new ODocument();
                document.OClassName = rune.Name;
                
                ODocument createdDocument = _database
                    .Create.Document(document)
                    .Run();
            }
        }

        public void Dispose()
        {
            _database.Close();   
        }
    }
}

