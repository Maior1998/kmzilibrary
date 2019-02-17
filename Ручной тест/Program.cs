using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;
using KMZILib;
using Vector = KMZILib.Vector;

namespace Ручной_тест
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string[] numbs = File.ReadAllLines(@"C:\Users\maior\Downloads\numbers.txt", Encoding.Default);

            double[] Test = numbs.Select(Convert.ToDouble).ToArray();

            MathStatistics.RandomValue value = new MathStatistics.RandomValue(Test);
            Console.WriteLine(value.DispersionQ);
        }

        
    }
}
