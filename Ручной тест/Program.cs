using KMZILib;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using LevenshteinCoding = KMZILib.CodingTheory.DataCompressionCodes.LevenshteinCoding;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
                Console.WriteLine(LevenshteinCoding.Decode(LevenshteinCoding.GetCode(14)));
        }
    }
}
