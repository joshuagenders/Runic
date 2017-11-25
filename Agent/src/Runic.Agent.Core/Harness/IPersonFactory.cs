using Runic.Agent.Core.Models;

namespace Runic.Agent.Core.Harness
{
    public interface IPersonFactory
    {
        IPerson GetPerson(Journey journey);
    }
}