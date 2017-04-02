﻿using Runic.Agent.Console.Framework;
using Runic.Agent.Service;
using System;
using System.Threading;

namespace Runic.Agent.Console.Pages
{
    public class ExecuteThreadPatternPage : MenuPage
    {
        public ExecuteThreadPatternPage(MenuProgram program)
            : base("Main Menu", program,
                  new Option("Execute Graph Thread Pattern", () => program.NavigateTo<ExecuteGraphPatternPage>()),
                  new Option("Execute Gradual Pattern", () => program.NavigateTo<ListAssembliesPage>()),
                  new Option("Execute Constant Pattern", () => program.NavigateTo<ListFunctionsPage>()))
        {
        }
    }
}