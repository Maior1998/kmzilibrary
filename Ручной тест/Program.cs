using KMZILib;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using Ciphers = KMZILib.Ciphers;
using Languages = KMZILib.Ciphers.Languages;
using Polynoms = KMZILib.Polynoms;
using ModularPolynom = KMZILib.Polynoms.ModularPolynom;
using DataCompressionCodes = KMZILib.CodingTheory.DataCompressionCodes;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(CodingTheory.DataCompressionCodes.LevenshteinCoding.GetCode(14));
        }
    }
}
