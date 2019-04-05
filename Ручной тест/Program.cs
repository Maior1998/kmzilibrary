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
            Polynoms.Polynom test = new Polynoms.Polynom("x^3 -0.1x^2 +0.3x-0.6");
            Console.WriteLine(test);
            Console.WriteLine(test.Derivative());
            Console.WriteLine(test.Derivative().Derivative());
            Console.WriteLine(test.Derivative().Derivative().Derivative());
            Console.WriteLine(test.Derivative().Derivative().Derivative().Derivative());

        }

        


    }
}


