using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Cucumber
{
    public partial class CucumberTest
    {
        private bool _hasFeature { get; set; } = false;
        private bool _hasScenario { get; set; } = false;

        private void CheckAddDescriptions()
        {
            if (!_hasFeature)
                Feature("Test Feature");
            if (!_hasScenario)
                Scenario("Test Scenario");
        }

        private void AppendStatement(string statementType, string statement)
        {
            var format = statement.Trim().ToLowerInvariant().StartsWith(statementType.ToLowerInvariant())
                ? "{0}\n"
                : statementType + " {0}\n";
            _stringBuilder.AppendFormat(format, statement);
        }

        public CucumberTest Feature(string statement)
        {
            _hasFeature = true;
            AppendStatement("Feature:", statement);
            return this;
        }
        public CucumberTest Background(string statement)
        {
            AppendStatement("Background:", statement);
            return this;
        }
        public CucumberTest Scenario(string statement)
        {
            _hasScenario = true;
            CheckAddDescriptions();
            AppendStatement("Scenario:", statement);
            return this;
        }
        public CucumberTest ScenarioOutline(string statement)
        {
            _hasScenario = true;
            CheckAddDescriptions();
            AppendStatement("Scenario Outline:", statement);
            return this;
        }
        public CucumberTest Given(string statement)
        {
            CheckAddDescriptions();
            AppendStatement("Given", statement);   
            return this;
        }
        public CucumberTest When(string statement)
        {
            CheckAddDescriptions();
            AppendStatement("When", statement);
            return this;
        }
        public CucumberTest Then(string statement)
        {
            CheckAddDescriptions();
            AppendStatement("Then", statement);
            return this;
        }
        public CucumberTest And(string statement)
        {
            CheckAddDescriptions();
            AppendStatement("And", statement);
            return this;
        }
        public CucumberTest But(string statement)
        {
            CheckAddDescriptions();
            AppendStatement("But", statement);
            return this;
        }

        public CucumberTest Examples(Dictionary<string,List<string>> table)
        {
            if (table.Count == 0)
                return this;
            var headers = table.Keys.ToList();
            _stringBuilder.AppendLine("Examples:");
            AddLine(headers);
            
            var rowCount = table[headers[0]].Count;
            for (var row = 0; row < rowCount; row++)
            {
                var line = new List<string>();
                foreach (var header in headers)
                {
                    line.Add(table[header][row]);
                }
                AddLine(line);
            }
            return this;
        }

        private void AddLine(List<string> lineValues)
        {
            Action<string> addValue = (a) => _stringBuilder.Append($" {a} |");
            Action<string> addValueLine = (a) => _stringBuilder.Append($"| {a} |");
            addValueLine(lineValues[0]);
            for (var i = 1; i < lineValues.Count; i++)
            {
                addValue(lineValues[i]);
            }
            _stringBuilder.AppendLine();
        }

        /// <summary>
        /// Takes format
        /// list[0] = header row
        /// list[1] = separator row
        /// list[2..n] = value rows
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public CucumberTest Examples(List<string> table)
        {
            if (table.Count < 3)
                return this;
            _stringBuilder.AppendLine("Examples:");
            table.ForEach(r => _stringBuilder.AppendLine(r));
            return this;
        }

        public CucumberTest DataTable(string table)
        {
            _stringBuilder.AppendLine("Examples:");
            _stringBuilder.AppendLine(table);
            return this;
        }
    }
}
