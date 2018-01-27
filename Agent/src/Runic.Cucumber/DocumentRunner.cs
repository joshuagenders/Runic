using Gherkin;
using Gherkin.Ast;
using Gherkin.Pickles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Runic.Cucumber
{
    public class DocumentRunner
    {
        private readonly IAssemblyAdapter _assemblyAdapter;
        public DocumentRunner(IAssemblyAdapter assemblyAdapter)
        {
            _assemblyAdapter = assemblyAdapter;
        }

        public TestResult Execute(string document)
        {
            List<Pickle> pickles;
            var testResult = new TestResult();
            try
            {
                var gherkinDoc = ParseTokenString(document);
                pickles = GetPickles(gherkinDoc);
                testResult.Steps = pickles?.SelectMany(p => p.Steps.Select(s => s.Text)).ToList();
            }
            catch (Exception ex)
            {
                testResult.Success = false;
                testResult.Exception = new GherkinDocumentParserError("The document could not be parsed", ex);
                return testResult;
            }
            if (pickles == null)
            {
                testResult.Success = false;
                testResult.Exception = new GherkinDocumentParserError("The document could not be parsed");
                return testResult;
            }
            foreach (var pickle in pickles)
            {
                foreach (var step in pickle.Steps)
                {
                    try
                    {
                        ExecuteStep(step);
                    }
                    catch (Exception ex)
                    {
                        testResult.FailedStep = step.Text;
                        testResult.Success = false;
                        testResult.Exception = ex;
                        return testResult;
                    }
                }
            }
            testResult.Success = true;
            return testResult;
        }

        public async Task<TestResult> ExecuteAsync(string document)
        {
            List<Pickle> pickles;
            var testResult = new TestResult();
            try
            {
                var gherkinDoc = ParseTokenString(document);
                pickles = GetPickles(gherkinDoc);
                testResult.Steps = pickles?.SelectMany(p => p.Steps.Select(s => s.Text)).ToList();
            }
            catch (Exception ex)
            {
                testResult.Success = false;
                testResult.Exception = new GherkinDocumentParserError("The document could not be parsed", ex);
                return testResult;
            }
            if (pickles == null)
            {
                testResult.Success = false;
                testResult.Exception = new GherkinDocumentParserError("The document could not be parsed");
                return testResult;
            }
            foreach (var pickle in pickles)
            {
                foreach (var step in pickle.Steps)
                {
                    try
                    {
                        await ExecuteStepAsync(step);
                    }
                    catch (Exception ex)
                    {
                        testResult.FailedStep = step.Text;
                        testResult.Success = false;
                        testResult.Exception = ex;
                        return testResult;
                    }
                }
            }
            testResult.Success = true;
            return testResult;
        }

        public void ExecuteStep(PickleStep step)
        {
            string[] arguments = step.Arguments
                                     .ToList()
                                     .Cast<PickleString>()
                                     .Select(a => a.Content)
                                     .ToArray();

            _assemblyAdapter.ExecuteMethodFromStatement(step.Text, arguments);
        }

        public async Task ExecuteStepAsync(PickleStep step)
        {
            string[] arguments = step.Arguments
                                     .ToList()
                                     .Cast<PickleString>()
                                     .Select(a => a.Content)
                                     .ToArray();

            await _assemblyAdapter.ExecuteMethodFromStatementAsync(step.Text, arguments);
        }

        public GherkinDocument ParseTokenString(string statements)
        {
            return new Parser().Parse(new TokenScanner(new StringReader(statements)),
                                                       new TokenMatcher());
        }

        public List<Pickle> GetPickles(GherkinDocument document)
        {
            return new Compiler().Compile(document);
        }
    }
}
