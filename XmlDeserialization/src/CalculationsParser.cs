using System;
using System.Collections.Generic;
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
            calculations = new List<Calculation>();
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

            for (;(line = ReadLine()) != null;)
            {  
                if (IsCalculationItemStart(line))
                {
                    for (var i = 0; i < 3; i++)
                    {
                        line = ReadLine(); 
                        if (line != null)
                        {
                            calcItem[i] = line;    
                        }
                    }

                    line = ReadLine();
                    if (IsCalculationItemEnd(line))
                    {
                        var calc = ReadCalculation(calcItem);
                        if (calc != null)
                        {
                            calculations.Add(calc);
                            Calculate(calc);
                        }
                        else
                        {
                            Console.WriteLine($"Error in file {FileName}: {ErrorMessage}, lines {lineNum-4}-{lineNum}");
                        }
                    }
                }
            }  
        }
        
#endregion PUBLIC_METHODS

#region PRIVATE_METHODS

        private static bool IsCalculationItemStart(string str)
        {
            return str.Contains("<folder name=\"calculation\">");
        }

        private static bool IsCalculationItemEnd(string str)
        {
            return str.Contains("</folder>");
        }

        private Calculation ReadCalculation(string[] calcItem)
        {
            var UidRgx = new Regex("<str +name=\"uid\" +value=\"([0-9a-f]*)\" *>"); // FIXME: */> ???
            
            if (!UidRgx.IsMatch(calcItem[0]))
            {
                ErrorMessage = $"Expected '<str name=\"uid\" value=\"...\">', but found '{calcItem[0]}'";
                return null;
            }
            var uid = UidRgx.Match(calcItem[0]).Groups[1].Value;         
            
            var OpRgx = new Regex("<str +name=\"operand\" +value=\"([a-z]*)\" */>");
            
            if (!OpRgx.IsMatch(calcItem[1]))
            {
                ErrorMessage = $"Expected '<str name=\"operand\" value=...>', but found '{calcItem[1]}'";
                return null;
            }      
            var op = OperationMethods.Parse(OpRgx.Match(calcItem[1]).Groups[1].Value);
            
            var ModRgx = new Regex("<int +name=\"mod\" +value=\"([0-9]*)\" */>");

            if (!ModRgx.IsMatch(calcItem[2]))
            {
                ErrorMessage = $"Expected '<int name=\"mod\" value=\"...\">', but found '{calcItem[2]}'";
                return null;
            }
            var mod = int.Parse(ModRgx.Match(calcItem[2]).Groups[1].Value);
            
            return new Calculation(uid, op, mod);
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
        
        private List<Calculation> calculations;
        public int ProcessedItems => calculations.Count;

        public string FileName { get; private set; }
        public int Result { get; private set; }
        public string ErrorMessage { get; private set; }
        
#endregion FIELDS_PROPS  
        
    }
}