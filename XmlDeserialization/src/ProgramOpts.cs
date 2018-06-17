using System;
using Mono.Options;

namespace XmlDeserialization
{
    public class ProgramOpts
    {
        public static string InputDirectory = ".";
        private static bool NeedShowHelp = false;
        
        private static OptionSet optSet = new OptionSet()
        {
            {
                "d|dir=", "Input directory name (Default directory is a current directory)",
                v => InputDirectory = v
            },
            {
                "h|help", "Show help",
                v => NeedShowHelp = !string.IsNullOrEmpty(v)
            }
        };
        
        public static void ParseArgs(string[] args)
        {
            optSet.Parse(args);
            if (NeedShowHelp) throw new OptionException("-- Help Message --", "h|help");
        }

        public static void HelpMessage()
        {
            Console.WriteLine("Usage: XmlDeserialization.exe [OPTIONS]");
            Console.WriteLine("Options: ");
            optSet.WriteOptionDescriptions(Console.Out);
        }
    }
}