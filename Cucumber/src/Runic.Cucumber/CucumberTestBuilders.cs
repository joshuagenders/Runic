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
            AppendStatement("Scenario:", statement);
            return this;
        }
        public CucumberTest ScenarioOutline(string statement)
        {
            _hasScenario = true;
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
    }
}
