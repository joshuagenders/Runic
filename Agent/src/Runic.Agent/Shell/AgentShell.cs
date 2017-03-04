using System;
using System.Linq;
using System.Threading;

namespace Runic.Agent.Shell
{
    public class AgentShell
    {
        public int ProcessCommands(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var input = Console.ReadLine().Split();
                    if (input.Any())
                    {
                        switch (input[0])
                        {
                            case "run":
                                SetThreadLevel(input);
                                break;
                        }
                    }
                }
                catch
                {
                    return 1;
                }

            }

            return 0;
        }

        private void SetThreadLevel(string[] input)
        {
            //todo
        }
    }
}
