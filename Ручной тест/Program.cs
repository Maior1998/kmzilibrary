﻿using KMZILib;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KMZILib.Ciphers;
using CodingTheory = KMZILib.CodingTheory;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            string encoded = DKS.Encode("МЕСТОВСТРЕЧИИЗМЕНИТЬНЕВОЗМОЖНО", "ПРИВЕТ", "ВЕСНА");
            Console.WriteLine(DKS.Decode(encoded, "ПРИВЕТ", "ВЕСНА"));
        }
    }
}
