using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.Core.TaskManagement
{
    public abstract class AbstractTask
    {
        public ICommand FailCommand { get; set; }
        public ICommand SuccessCommand { get; set; }
        public ICommand ProgressCommand { get; set; }

        protected string Message { get; set; }

        public void Fail()
        {
            FailCommand.Execute();
        }

        public void Succeed()
        {
            SuccessCommand.Execute();
        }

        public void ShowProgress()
        {
            ProgressCommand.Execute();
        }

        public string GetMessage() => Message;
        public abstract void Run();
    }
}
