using System;

namespace XmlDeserialization
{
    public enum Operation
    {
        Unknown,
        Add,
        Multiply,
        Divide,
        Substract
    }

    internal static class OperationMethods
    {

        public static Operation Parse(string str)
        {
            switch (str.ToLower())
            {
                case "add":
                    return Operation.Add;
                case "subtract":
                    return Operation.Substract;
                case "multiply":
                    return Operation.Multiply;
                case "divide":
                    return Operation.Divide;
                default:
                    return Operation.Unknown;
            }
        }
    }
   
    public class Calculation
    {
        public Calculation(string uid, Operation op, int mod)
        {
            this.Uid = uid;
            this.Op = op;
            this.Mod = mod;
        }

        public string Uid { get; set; }
        public Operation Op { get; set; }
        public int Mod { get; set; }

        public override string ToString()
        {
            return $"{{{Uid}, {Op}, {Mod}}}";
        }
    }
    
}