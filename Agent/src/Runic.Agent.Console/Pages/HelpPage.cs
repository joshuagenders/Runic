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
            Output.WriteLine(ConsoleColor.DarkCyan, "==========================");
            Output.WriteLine(ConsoleColor.DarkYellow, "        Runic Agent       ");
            Output.WriteLine(ConsoleColor.DarkCyan, "==========================");
            Output.WriteLine(ConsoleColor.DarkGreen, "          Help         ");
            Output.WriteLine(ConsoleColor.DarkCyan, "--------------------------");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nList Assemblies");
            Output.WriteLine(ConsoleColor.White, "Lists all loaded assemblies");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nList Functions");
            Output.WriteLine(ConsoleColor.White, "Lists all available functions in all loaded assemblies");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nLoad Assembly");
            Output.WriteLine(ConsoleColor.White, "Loads an assembly using a key and the configured plugin provider");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nLoad Flow");
            Output.WriteLine(ConsoleColor.White, "Loads a flow into the configured flow manager from a json file");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nSet Flow Thread Count");
            Output.WriteLine(ConsoleColor.White, "Sets the thread count on the given flow");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nList Running Flows");
            Output.WriteLine(ConsoleColor.White, "Lists all running flows in the agent");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nList Running Thread Patterns");
            Output.WriteLine(ConsoleColor.White, "Lists all running thread patterns in the agent");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nStop Flow");
            Output.WriteLine(ConsoleColor.White, "Sets the thread level to 0");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nDisplay Agent Information");
            Output.WriteLine(ConsoleColor.White, "Displays detailed information about the agent state");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nExecute Thread Pattern");
            Output.WriteLine(ConsoleColor.White, "Executes a given thread pattern against a given flow");
            Output.WriteLine(ConsoleColor.DarkCyan, "\nStop Thread Pattern");
            Output.WriteLine(ConsoleColor.White, "Cancels a given thread pattern");

            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}