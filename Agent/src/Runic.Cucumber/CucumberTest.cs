using System.Reflection;
using System.Text;

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

        public TestResult Execute()
        {
            var doc = _stringBuilder.ToString();
            return _documentRunner.Execute(doc);
        }
    }
}
