﻿using System;

namespace Runic.Agent.Console.Framework
{
    public static class Output
    {
        public static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine("  " + format, args);
            System.Console.ResetColor();
        }

        public static void WriteLine(ConsoleColor color, string value)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine("  " + value);
            System.Console.ResetColor();
        }

        public static void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine("  " + format, args);
        }

        public static void DisplayPrompt(string format, params object[] args)
        {
            format = format.Trim() + " ";
            System.Console.Write("  " + format, args);
        }
    }
}
