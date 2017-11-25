using Runic.Agent.Core.Models;
using Runic.Agent.Core.Services;

namespace Runic.Agent.Core.Harness
{
    public interface IPersonFactory
    {
        IPerson GetPerson(Journey journey);
    }
}