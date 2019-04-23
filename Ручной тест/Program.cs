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
            BinaryFunction a = new BinaryFunction(0b11001011, 3);
            Console.WriteLine(a.ZhegalkinPolynomial);
            Console.WriteLine(a.Degree);

        }

        


    }
}


