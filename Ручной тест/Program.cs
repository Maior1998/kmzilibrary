﻿using KMZILib;

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
            Console.WriteLine(BinaryFunction.GetDerivativeInDirection(new BinaryFunction(0b0110, 2), new[] {false, true}).Value);
        }

        


    }
}


