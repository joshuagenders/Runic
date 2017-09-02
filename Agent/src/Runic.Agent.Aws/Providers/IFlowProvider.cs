using Runic.Framework.Models;
using System.Threading.Tasks;

namespace Runic.Agent.Aws.Providers
{
    public interface IFlowProvider
    {
        Task<Flow> GetFlow(string key);
    }
}
