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
              BinaryFunction test = new BinaryFunction(0b1001,2);
              BinaryFunction[] test2 = new[]
              {
                  new BinaryFunction(0b0110,2),
                  new BinaryFunction(0b1101,2),
                  new BinaryFunction(0b0100,2)
              };
              Console.WriteLine(test.Distance(test2));

        }

        


    }
}


