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
            int length = 6;
            Console.WriteLine(Math.Pow(2,new int[32].Select((val,ind)=>ind+1).First(val=>Math.Pow(2,val)>=length)));

        }

        


    }
}


