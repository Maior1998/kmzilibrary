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

namespace Ручной_тест
{
    class Program
    {

        static void Main(string[] args)
        {
            Matrix a = new Matrix(
                new[]
                {
                    new [] { 0, 0, 1, 1 },
                    new [] { 0, 0, 1, 0 },
                    new [] {0,0,1,1},
                    new [] {0,1,1,0}
                });
            Matrix b = new Matrix(
                new[]
                {
                    new[] { 1, 1, 1 },
                    new[] { 1, 1, 0 },
                    new[] { 1, 1, 0}
                });

            Graph graphA = Graph.GetGraph(a);
            Graph graphB = Graph.GetGraph(b);


            Console.WriteLine(graphA.Join(graphB).AdjacencyMatrix);

            Console.WriteLine();
        }
    }
}
