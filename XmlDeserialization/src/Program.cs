using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XmlDeserialization
{

    class Program
    {
        
        public static void Main(string[] args)
        {
            ProgramOpts.ParseArgs(args);
            if (ProgramOpts.Check()){ ProgramOpts.HelpMessage(); return; }
            
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
            
            Console.WriteLine("Tasks count: " + tasksToProcess.Count);
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            foreach (var t in tasksToProcess)
            {
                t.Start();
            }

            Task.WaitAll(tasksToProcess.ToArray());

            watch.Stop();

            var max = parsers.Max(p => p.ProcessedItems);
            var mp = parsers.First(p => p.ProcessedItems == max);

            Console.WriteLine($"{mp.FileName}: processed items = {mp.ProcessedItems}, result = {mp.Result}");
            Console.WriteLine($"Elapced time is {watch.ElapsedMilliseconds.ToString()} ms");
        }
    }
}