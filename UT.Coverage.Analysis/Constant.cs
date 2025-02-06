using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.Coverage.Analysis
{
    public static class Constant
    {
        public static string NOT = "!";
        public static string OR = "||";
        public static string AND = "&&";
        public static string CHILDFLG = "$Child$";
        public static string FALSE = "(NOT)";
        public static string REPLACEFLGSTART = "{";
        public static string REPLACEFLGEND = "}";
        public static List<string> SPECTIALMETHOD = new() 
        {
            ".IsNullOrEmpty(",
            ".StartsWith(",
            ".Substring("
        };
    }
}
