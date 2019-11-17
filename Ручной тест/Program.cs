using KMZILib;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using RangeConding=KMZILib.CodingTheory.DataCompressionCodes.EntropyCoding.RangeCoding;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(RangeConding.Encode("AABA",100000));
        }
    }
}
