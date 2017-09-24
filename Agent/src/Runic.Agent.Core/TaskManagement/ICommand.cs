using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.Core.TaskManagement
{
    public interface ICommand
    {
        void Execute();
    }
}
