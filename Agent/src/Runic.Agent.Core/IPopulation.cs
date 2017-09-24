using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public interface IPopulation
    {
        Task<bool> RequestPerson();
        void ReleasePerson();
    }
}