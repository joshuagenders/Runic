using Runic.Agent.Console.Framework;
using System;

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
                  new Option("List Running Thread Patterns", () => program.NavigateTo<ListRunningThreadPatternsPage>()),
                  new Option("Stop Thread Pattern", () => program.NavigateTo<StopFlowPage>()),
                  new Option("Help", () => program.NavigateTo<HelpPage>()),
                  new Option("Display Agent Information", () => program.NavigateTo<DisplayAgentInformationPage>()),
                  new Option("Exit", () => program.NavigateTo<ExitPage>()))
        {
        }

        public override void Display()
        {
            Output.WriteLine(ConsoleColor.DarkCyan, @"    ____              _     ");
            Output.WriteLine(ConsoleColor.DarkCyan, @"   / __ \__  ______  (_)____");
            Output.WriteLine(ConsoleColor.DarkCyan, @"  / /_/ / / / / __ \/ / ___/");
            Output.WriteLine(ConsoleColor.DarkCyan, @" / _, _/ /_/ / / / / / /__  ");
            Output.WriteLine(ConsoleColor.DarkCyan, @"/_/ |_|\__,_/_/ /_/_/\___/  ");
            Output.WriteLine(ConsoleColor.DarkMagenta, "____________________________\n");

            base.Display();
        }
    }
}
