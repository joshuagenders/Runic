using Runic.Core;
using System.Collections.Generic;
using Orient.Client;
using OrientDB_Net.binary.Innov8tive.API;
using System;
using System.Reflection;
using Runic.Core.Models;

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
                document = TransferProperties(document, rune);

                ODocument createdDocument = _database
                    .Create.Document(document)
                    .Run();
            }
        }

        private ODocument TransferProperties<T>(ODocument document, T rune)
        {
            foreach (var prop in rune.GetType().GetProperties())
            {
                document.Add(prop.Name, prop.GetValue(rune));
            }
            return document;
        }

        public void Dispose()
        {
            _database.Close();   
        }
    }
}

