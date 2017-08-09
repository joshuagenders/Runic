using Runic.Framework.Models;

namespace Runic.Agent.Core.ExternalInterfaces
{
    public interface IDataStore
    {
        void StoreRune<T>(T rune) where T : Rune;
        T GetRune<T>() where T : Rune;
        T GetRune<T>(RuneQuery query) where T : Rune;
        T UseRune<T>(T rune) where T : Rune;
    }
}
