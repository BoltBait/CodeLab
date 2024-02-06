/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PdnCodeLab
{
    public static class ProcessUtil
    {
        public static int TryExec(string fileName, string commandLine)
        {
            Console.WriteLine($"Execute: '{fileName}' '{commandLine}'");
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, commandLine);
            startInfo.UseShellExecute = !Path.IsPathFullyQualified(fileName) || !Path.GetExtension(fileName).Equals(".exe", StringComparison.OrdinalIgnoreCase);
            using Process process = Process.Start(startInfo)!;
            process.WaitForExit();
            return process.ExitCode;
        }

        public static void Exec(string fileName, string commandLine)
        {
            int exitCode = TryExec(fileName, commandLine);
            if (exitCode != 0)
            {
                throw new ApplicationException($"'{fileName}' returned {exitCode}. Command line: {commandLine}");
            }
        }

        public static void Exec(string fileName, IEnumerable<string> args)
        {
            int exitCode = TryExec(fileName, args);
            if (exitCode != 0)
            {
                string commandLine = ConvertArgsToCommandLine(args);
                throw new ApplicationException($"'{fileName}' returned {exitCode}. Command line: {commandLine}");
            }
        }

        public static void Exec(string fileName, params string[] args)
        {
            Exec(fileName, (IEnumerable<string>)args);
        }

        public static int TryExec(string fileName, IEnumerable<string> args)
        {
            string commandLine = ConvertArgsToCommandLine(args);
            return TryExec(fileName, commandLine);
        }

        public static int TryExec(string fileName, params string[] args)
        {
            return TryExec(fileName, (IEnumerable<string>)args);
        }

        private static string ConvertArgsToCommandLine(IEnumerable<string> args)
        {
            string[] escapifiedArgs = EscapifyArgs(args).ToArray();
            string commandLine = escapifiedArgs.Join(" ");
            return commandLine;
        }

        private static IEnumerable<string> EscapifyArgs(IEnumerable<string> args)
        {
            foreach (string arg in args)
            {
                yield return EscapifyArg(arg);
            }
        }

        private static string EscapifyArg(string arg)
        {
            StringBuilder resultBuilder = new StringBuilder();

            bool quotes = false;
            for (int i = 0; i < arg.Length; ++i)
            {
                if (char.IsWhiteSpace(arg[i]))
                {
                    quotes = true;
                    break;
                }
            }

            if (quotes)
            {
                resultBuilder.Append('\"');
            }

            for (int i = 0; i < arg.Length; ++i)
            {
                char c = arg[i];
                if (c == '\"')
                {
                    // double-quotes
                    resultBuilder.Append('\"');
                }
                else if (quotes && c == '\\' && i == arg.Length - 1)
                {
                    // make sure backslash at end doesn't escapify the ending quote
                    resultBuilder.Append('\\');
                }

                resultBuilder.Append(c);
            }

            if (quotes)
            {
                resultBuilder.Append('\"');
            }

            return resultBuilder.ToString();
        }
    }
}
