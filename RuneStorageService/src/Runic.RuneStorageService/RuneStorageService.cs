using System;
using System.Collections.Generic;
using System.Reflection;
using Orient.Client;
using OrientDB_Net.binary.Innov8tive.API;
using Runic.Core.Models;

namespace Runic.RuneStorageService
{
    public class RuneStorageService : IDisposable
    {
        public RuneStorageService()
        {
            _database = new ODatabase(new ConnectionOptions());
        }

        private ODatabase _database { get; }

        public void Dispose()
        {
            _database.Close();
        }

        public void StoreRunes<T>(List<T> runes) where T : Rune
        {
            foreach (var rune in runes)
            {
                var document = new ODocument();
                document.OClassName = rune.Name;
                document = TransferProperties(document, rune);

                var createdDocument = _database
                    .Create.Document(document)
                    .Run();
            }
        }

        private ODocument TransferProperties<T>(ODocument document, T rune)
        {
            foreach (var prop in rune.GetType().GetProperties())
                document.Add(prop.Name, prop.GetValue(rune));
            return document;
        }
    }
}