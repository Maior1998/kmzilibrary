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
        public static double[][] Test = new[]
        {
            new[] {1.0, 4, 7},
            new[] {3.0, 0, 5},
            new[] {-1.0, 9, 11}
        };
        static void Main(string[] args)
        {
            Console.WriteLine(Matrix.GetDefinite(new Matrix( Test)));  

        }

        


    }
}


