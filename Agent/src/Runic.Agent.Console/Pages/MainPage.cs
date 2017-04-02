using Runic.Agent.Console.Framework;

namespace Runic.Agent.Console.Pages
{
    class MainPage : MenuPage
    {
        public MainPage(MenuProgram program)
            : base("Main Menu", program,
                  new Option("Load Assembly", () => program.NavigateTo<LoadAssemblyPage>()),
                  new Option("List Assemblies", () => program.NavigateTo<ListAssembliesPage>()),
                  new Option("List Functions", () => program.NavigateTo<ListFunctionsPage>()),
                  new Option("Load Flow", () => program.NavigateTo<LoadFlowPage>()),
                  new Option("Set Flow Thread Level", () => program.NavigateTo<SetThreadPage>()),
                  new Option("List Running Flows", () => program.NavigateTo<ListRunningFlowsPage>()),
                  new Option("Stop Flow", () => program.NavigateTo<StopFlowPage>()),
                  new Option("Execute Thread Pattern", () => program.NavigateTo<ExecuteThreadPatternPage>()),
                  new Option("Stop Thread Pattern", () => program.NavigateTo<StopFlowPage>()),
                  new Option("Help", () => program.NavigateTo<HelpPage>()),
                  new Option("Display Agent Information", () => program.NavigateTo<DisplayAgentInformationPage>()),
                  new Option("Exit", () => program.NavigateTo<ExitPage>()))
        {
        }
    }
}
