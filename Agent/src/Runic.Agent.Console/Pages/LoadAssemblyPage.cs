using Runic.Agent.Console.Framework;
using Runic.Agent.Core.AssemblyManagement;
using System;
using System.Linq;

namespace Runic.Agent.Console.Pages
{
    class LoadAssemblyPage : Page
    {
        private readonly IPluginManager _pluginManager;
        public LoadAssemblyPage(MenuProgram program, IPluginManager pluginManager)
            : base("Load Plugin", program)
        {
            _pluginManager = pluginManager;
        }

        public override void Display()
        {
            base.Display();

            var input = Input.ReadString("Enter the assembly plugin key");
            _pluginManager.LoadPlugin(input);

            if (_pluginManager.GetAssemblyKeys().Any(a => a == input))
            { 
                Output.WriteLine(ConsoleColor.Green, $"Assembly loaded", input);
            }
            else
            {
                Output.WriteLine(ConsoleColor.Red, $"Assembly load failed for plugin key {input}", input);
            }
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
