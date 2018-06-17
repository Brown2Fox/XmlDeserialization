using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace XmlDeserialization
{
    public class CalculationsParser
    {

#region CTORS
        
        public CalculationsParser(string fileName)
        {
            FileName = fileName;
            Calculations = new List<Calculation>();
        }
        
#endregion CTORS

#region PUBLIC_METHODS
        
        public void Parse()
        {
            var calcItem = new string[3];
            
            var file = new StreamReader(FileName);
            
            var lineNum = 0;
            var line = "";

            string ReadLine() { lineNum++; return file.ReadLine(); }

            while ((line = ReadLine()) != null)
            {  
                if (IsCalculationItemStart(line))
                {
                    var uid = ReadUid(ReadLine());
                    if (uid == null)
                    {
                        PrintError(lineNum); continue;
                    }

                    var op = ReadOperation(ReadLine());
                    if (op == null)
                    {
                        PrintError(lineNum); continue;
                    }

                    var mod = ReadMod(ReadLine());
                    if (mod == null)
                    {
                        PrintError(lineNum); continue;
                    }

                    if (IsCalculationItemEnd(ReadLine()))
                    {
                        var calc = new Calculation(uid, OperationMethods.Parse(op), int.Parse(mod));
                        Calculations.Add(calc);
                        Calculate(calc);
                    }
                    else
                    {
                        PrintError(lineNum);
                    }
                }
            }  
        }
        
#endregion PUBLIC_METHODS

#region PRIVATE_METHODS

        private void PrintError(int lineNum)
        {
            Console.WriteLine($"Error in file {Path.GetFileName(FileName)}: {ErrorMessage}, line {lineNum}");   
        }
        
        private bool IsCalculationItemStart(string line)
        {
            var StartRgx = new Regex("<folder +name=\"calculation\" *>");
            return !string.IsNullOrEmpty(line) && StartRgx.IsMatch(line);
        }

        private bool IsCalculationItemEnd(string line)
        {
            var EndRgx = new Regex("(</folder>)");
            return null != ReadItem(EndRgx, line);
        }

        private string ReadUid(string line)
        {
            var UidRgx = new Regex("<str +name=\"uid\" +value=\"([0-9a-f]*)\" *>"); // FIXME: */> ?
            return ReadItem(UidRgx, line);
        }

        private string ReadOperation(string line)
        {              
            var OpRgx = new Regex("<str +name=\"operand\" +value=\"([a-z]*)\" */>");
            return ReadItem(OpRgx, line);
        }

        private string ReadMod(string line)
        {
            var ModRgx = new Regex("<int +name=\"mod\" +value=\"([0-9]*)\" */>");
            return ReadItem(ModRgx, line);
        }

        private string ReadItem(Regex rgx, string line)
        {
            if (string.IsNullOrEmpty(line) || !rgx.IsMatch(line))
            {
                var found = !string.IsNullOrEmpty(line) ? line.TrimStart(' ') : "empty";
                ErrorMessage = $"Expected '{rgx}', but found '{found}'";
                return null;
            }
            return rgx.Match(line).Groups[1].Value;
        }
        
        private void Calculate(Calculation calc)
        {
            switch (calc.Op)
            {
                case Operation.Add:
                    Result += calc.Mod;
                    break;
                case Operation.Substract:
                    Result -= calc.Mod;
                    break;
                case Operation.Multiply:
                    Result *= calc.Mod;
                    break;
                case Operation.Divide:
                    Result /= calc.Mod;
                    break;
            }
        }
        
#endregion PRIVATE_METHODS
        
#region FIELDS_PROPS
        
        public List<Calculation> Calculations { get; }      

        public string FileName { get; private set; }
        public int Result { get; private set; }
        public string ErrorMessage { get; private set; }
        
#endregion FIELDS_PROPS  
        
    }
}