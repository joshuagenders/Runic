using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.Services;

namespace Runic.Agent.Core
{
    public interface IPersonFactory
    {
        IPerson GetPerson(Journey journey);
    }
}