using Runic.Agent.AssemblyManagement;
using Runic.Agent.Console.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runic.Agent.Console.Pages
{
    public class ListAssembliesPage : Page
    {
        private readonly IPluginManager _pluginManager;

        public ListAssembliesPage(MenuProgram program, IPluginManager pluginManager) 
            : base("List Assemblies", program)
        {
            _pluginManager = pluginManager;
        }

        public override void Display()
        {
            base.Display();
            var assemblies = _pluginManager.GetAssemblies();
            assemblies?.ToList().ForEach(i =>
            {
                Output.WriteLine(ConsoleColor.Green, $"{i.FullName}");
            });
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
