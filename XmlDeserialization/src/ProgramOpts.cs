using System;
using Mono.Options;

namespace XmlDeserialization
{
    public class ProgramOpts
    {
        public static string InputDirectory;
        
        private static OptionSet optSet = new OptionSet()
        {
            {
                "d|dir=", "Input directory name.",
                v => InputDirectory = v
            }
        };
        
        public static void ParseArgs(string[] args)
        { 
           optSet.Parse(args);   
        }

        public static bool Check()
        {
            if (string.IsNullOrEmpty(InputDirectory)) return true;
            return false;
        }

        public static void HelpMessage()
        {
            Console.WriteLine("Usage: XmlDeserialization.exe [OPTIONS]");
            Console.WriteLine("Options: ");
            optSet.WriteOptionDescriptions(Console.Out);
        }
    }
}