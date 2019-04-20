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

namespace Ручной_тест
{
    class Program
    {
        
        static void Main(string[] args)
        {
            BinaryFunction test = new BinaryFunction(0b11011001, 3);
            Console.WriteLine(BinaryFunction.GetWalshHadamardTransformString(test));
            Console.WriteLine($"{string.Join("\n",BinaryFunction.WalshHadamardSpectrum(test))}");

        }

        


    }
}


