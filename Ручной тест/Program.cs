using KMZILib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Ciphers = KMZILib.Ciphers;
using Languages = KMZILib.Ciphers.Languages;
using Polynoms = KMZILib.Polynoms;
using ModularPolynom=KMZILib.Polynoms.ModularPolynom;
using DataCompressionCodes=KMZILib.CodingTheory.DataCompressionCodes;

namespace Ручной_тест
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string test = "bcbabbbbcc";
            Console.WriteLine(Misc.DoubleFractToString(CodingTheory.DataCompressionCodes.ArithmeticCoding.GetCode(test,out double AL),9));
        }


        
    }
}


