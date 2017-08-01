using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Cucumber
{
    public partial class CucumberTest
    {
        private readonly StringBuilder _stringBuilder;
        private readonly DocumentRunner _documentRunner;

        public CucumberTest(IAssemblyAdapter adapter)
        {
            _stringBuilder = new StringBuilder();
            _documentRunner = new DocumentRunner(adapter);
        }

        public CucumberTest(Assembly assembly)
        {
            _stringBuilder = new StringBuilder();
            var adapter = new AssemblyAdapter(assembly, new TestStateManager());
            _documentRunner = new DocumentRunner(adapter);
        }

        public async Task<bool> ExecuteAsync(string document, CancellationToken ctx = default(CancellationToken))
        {
            await _documentRunner.ExecuteAsync(document, ctx);
            return await Task.FromResult(true);
        }

        public async Task<bool> ExecuteAsync(CancellationToken ctx = default(CancellationToken))
        {
            var doc = _stringBuilder.ToString();
            await _documentRunner.ExecuteAsync(doc, ctx);
            return await Task.FromResult(true);
        }
    }
}
