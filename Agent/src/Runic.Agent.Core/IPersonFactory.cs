using Runic.Agent.Core.Models;
using Runic.Agent.Services;

namespace Runic.Agent.Core
{
    public interface IPersonFactory
    {
        IPerson GetPerson(Journey journey);
    }
}