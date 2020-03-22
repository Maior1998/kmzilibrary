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
using System.Threading.Tasks;
using KMZILib.Ciphers;
using CodingTheory = KMZILib.CodingTheory;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            string encoded = SKS.Encode("ВРАГБУДЕТРАЗБИТ", "ПАМИР");
            string decoded = SKS.Decode(encoded, "ПАМИР");
            Console.WriteLine();
        }
    }
}
