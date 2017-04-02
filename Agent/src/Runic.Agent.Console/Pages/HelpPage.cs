using Runic.Agent.Console.Framework;
using System;

namespace Runic.Agent.Console.Pages
{
    public class HelpPage : Page
    {
        public HelpPage(MenuProgram program) : base("Help", program)
        {
        }

        public override void Display()
        {
            base.Display();
            Output.WriteLine(ConsoleColor.White, "==========================");
            Output.WriteLine(ConsoleColor.White, "        Runic Agent       ");
            Output.WriteLine(ConsoleColor.White, "==========================");
            Output.WriteLine(ConsoleColor.DarkGreen, "Help");
            Output.WriteLine(ConsoleColor.White, "--------------------------");
            Output.WriteLine(ConsoleColor.White, "List Assemblies");
            Output.WriteLine(ConsoleColor.White, "Lists all loaded assemblies");
            Output.WriteLine(ConsoleColor.White, "List Functions");
            Output.WriteLine(ConsoleColor.White, "Lists all available functions in all loaded assemblies");
            Output.WriteLine(ConsoleColor.White, "Load Assembly");
            Output.WriteLine(ConsoleColor.White, "Loads an assembly using a key and the configured plugin provider");
            Output.WriteLine(ConsoleColor.White, "Load Flow");
            Output.WriteLine(ConsoleColor.White, "Loads a flow into the configured flow manager from a json file");
            Output.WriteLine(ConsoleColor.White, "Set Flow Thread Count");
            Output.WriteLine(ConsoleColor.White, "Sets the thread count on the given flow");
            Output.WriteLine(ConsoleColor.White, "List Running Flows");
            Output.WriteLine(ConsoleColor.White, "Lists all running flows in the agent");
            Output.WriteLine(ConsoleColor.White, "List Running Thread Patterns");
            Output.WriteLine(ConsoleColor.White, "Lists all running thread patterns in the agent");
            Output.WriteLine(ConsoleColor.White, "Stop Flow");
            Output.WriteLine(ConsoleColor.White, "Sets the thread level to 0");
            Output.WriteLine(ConsoleColor.White, "Execute Thread Pattern");
            Output.WriteLine(ConsoleColor.White, "Executes a given thread pattern against a given flow");
            Output.WriteLine(ConsoleColor.White, "Stop Thread Pattern");
            Output.WriteLine(ConsoleColor.White, "Cancels a given thread pattern");

            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}