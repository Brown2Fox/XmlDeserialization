using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mono.Options;

namespace XmlDeserialization
{

    class Program
    {
        
        public static void Main(string[] args)
        {
            try
            {
                ProgramOpts.ParseArgs(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                ProgramOpts.HelpMessage();
                return;
            }
            
            
            var XMLFileExtentionRgx = new Regex(@".+\.xml", RegexOptions.IgnoreCase);

            var tasksToProcess = new List<Task>();
            var parsers = new List<CalculationsParser>();
            
            var fileNames = Directory.GetFiles(ProgramOpts.InputDirectory);       
     
            foreach (var fileName in fileNames)
            {
                if (XMLFileExtentionRgx.IsMatch(fileName))
                {
                    var parser = new CalculationsParser(fileName);
                    parsers.Add(parser);
                    var task = new Task(() => { parser.Parse(); });
                    tasksToProcess.Add(task);
                }
            }
            
            Console.WriteLine("Files to process: " + tasksToProcess.Count);
            if (tasksToProcess.Count == 0) return;
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            foreach (var t in tasksToProcess)
            {
                t.Start();
            }

            Task.WaitAll(tasksToProcess.ToArray());

            watch.Stop();

            
            Console.WriteLine("--- ALL RESULTS ---");
            foreach (var p in parsers)
            {
                Console.WriteLine($"File: {Path.GetFileName(p.FileName)}, Result: {p.Result}, Processed Items: {p.Calculations.Count}");
            }
            
            var max = parsers.Max(p => p.Calculations.Count);
            var mps = parsers.FindAll(p => p.Calculations.Count == max);

            Console.WriteLine("--- BEST RESULTS ---");
            foreach (var mp in mps)
            {
                Console.WriteLine($"File: {Path.GetFileName(mp.FileName)}, Result: {mp.Result}, Processed Items: {mp.Calculations.Count}");    
            }
            Console.WriteLine($"Elapced time: {watch.ElapsedMilliseconds.ToString()} ms");
        }
    }
}