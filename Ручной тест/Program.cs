using OSULib;

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
using OSULib.Ciphers;
using OSULib.Maths;
using OSULib.Maths.Graphs;
using CodingTheory = OSULib.Misc.CodingTheory;
using OSULib.Maths.GamesTheory;

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            Matrix source = new Matrix(new double[][]
            {
                new double[]{5, -3, 6, -8, 7, 4},
                new double[]{7, 5, 5, -4, 8, 1},
                new double[]{1, 3, -1 ,10, 0 ,2},
                new double[]{9, -9, 7, 1, 3, 6},
            });
            GameWithNature gameWithNature = new GameWithNature(source);
            Console.WriteLine();
        }


    }
}
