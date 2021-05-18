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
            Matrix Vsource = new Matrix(new double[][]
            {
                new double[]{1, 0, -1, 1},
                new double[]{2, 2, -2, 0},
                new double[]{4, 1, 3, 2},
                new double[]{5, -1, 1, 1},
                new double[]{3,2,1,-2},
            });
            Matrix Wsource = new Matrix(new double[][]
            {
                new double[]{0, -0.5, -2, -4},
                new double[]{-1, 0.5, 1, -1},
                new double[]{-2.5, -1, 3.5, 3.5},
                new double[]{-3.5, -2, 2.5, 6.5},
            });
            GameWithNature gameWithNature = new GameWithNature(Wsource);
            Console.WriteLine();
        }


    }
}
