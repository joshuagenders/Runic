using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System;

namespace Runic.Agent.Console.Pages
{
    public class StopThreadPatternPage : Page
    {
        private readonly IAgentService _agentService;
        public StopThreadPatternPage(MenuProgram program, IAgentService agentService) 
            : base("Stop Thread Pattern", program)
        {
            _agentService = agentService;
        }
    }
}
