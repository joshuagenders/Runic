using System;
using System.Linq;

namespace Runic.Agent.Console.Framework
{
    public abstract class Page
    {
        public string Title { get; private set; }

        public MenuProgram MenuProgram { get; set; }

        public Page(string title, MenuProgram program)
        {
            Title = title;
            MenuProgram = program;
        }

        public virtual void Display()
        {
            if (MenuProgram.History.Count > 1 && MenuProgram.BreadcrumbHeader)
            {
                string breadcrumb = null;
                foreach (var title in MenuProgram.History.Select((page) => page.Title).Reverse())
                    breadcrumb += title + " > ";
                breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);
                System.Console.WriteLine(breadcrumb);
            }
            else
            {
                Output.WriteLine(ConsoleColor.DarkCyan, Title);
            }
            Output.WriteLine(ConsoleColor.DarkBlue, "---");
        }
    }
}
