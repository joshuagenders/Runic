﻿using Gherkin;
using Gherkin.Ast;
using Gherkin.Pickles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        public async Task ExecuteAsync(string document, CancellationToken ctx = default(CancellationToken))
        {
            var gherkinDoc = ParseTokenString(document);
            var pickles = GetPickles(gherkinDoc);
            foreach (var pickle in pickles)
            {
                foreach (var step in pickle.Steps)
                {
                    var stepTask = ExecuteStepAsync(step, ctx);
                    await stepTask;
                    if (stepTask.IsFaulted || stepTask.Exception != null)
                    {
                        //todo failure logic
                        throw stepTask.Exception;
                    }
                }
            }
        }

        public async Task ExecuteStepAsync(PickleStep step, CancellationToken ctx = default(CancellationToken))
        {
            string[] arguments = step.Arguments
                                     .ToList()
                                     .Cast<PickleString>()
                                     .Select(a => a.Content)
                                     .ToArray();

            await _assemblyAdapter.ExecuteMethodFromStatementAsync(step.Text, arguments, ctx);
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