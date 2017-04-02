using Newtonsoft.Json;
using Runic.Agent.Console.Framework;
using Runic.Agent.FlowManagement;
using Runic.Framework.Models;
using System;
using System.IO;

namespace Runic.Agent.Console.Pages
{
    class LoadFlowPage : Page
    {
        private readonly IFlowManager _flowManager;
        public LoadFlowPage(MenuProgram program, IFlowManager flowManager)
            : base("Input", program)
        {
            _flowManager = flowManager;
        }

        public override void Display()
        {
            base.Display();

            var input = Input.ReadString("Enter the flow filename");
            if (File.Exists(input))
            {
                var text = File.ReadAllText(input);
                var flow = JsonConvert.DeserializeObject<Flow>(text);
                _flowManager.AddUpdateFlow(flow);
                Output.WriteLine(ConsoleColor.Green, $"Flow added with name {flow.Name}", input);
            }
            else
            {
                Output.WriteLine(ConsoleColor.Red, $"File not found {input}", input);
            }
            Input.ReadString("Press [enter] to return");
            MenuProgram.NavigateHome();
        }
    }
}
