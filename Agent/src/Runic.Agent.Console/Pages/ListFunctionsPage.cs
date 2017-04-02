using Runic.Agent.AssemblyManagement;
using Runic.Agent.Console.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runic.Agent.Console.Pages
{
    public class ListFunctionsPage : Page
    {
        private readonly IPluginManager _pluginManager;
        public ListFunctionsPage(MenuProgram program, IPluginManager pluginManager) 
            : base("List Functions", program)
        {
            _pluginManager = pluginManager;
        }

        public override void Display()
        {
            base.Display();
            var functions = _pluginManager.GetAvailableFunctions();
            functions.ToList().ForEach(i =>
            {
                Output.WriteLine(ConsoleColor.Green, $"{i.FunctionName}");
            });
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
