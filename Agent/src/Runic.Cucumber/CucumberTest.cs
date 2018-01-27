using System.Reflection;
using System.Text;
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

        public TestResult Execute(string document)
        {
            return _documentRunner.Execute(document);
        }

        public async Task<TestResult> ExecuteAsync(string document)
        {
            return await _documentRunner.ExecuteAsync(document);
        }

        public TestResult Execute()
        {
            var doc = _stringBuilder.ToString();
            return _documentRunner.Execute(doc);
        }
    }
}
