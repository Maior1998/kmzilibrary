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
using CodingTheory = KMZILib.CodingTheory;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(CodingTheory.LZ77.Encode("У животных есть душа. Я видел это в их глазах.", 10));
        }
    }
}
