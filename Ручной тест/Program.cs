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
            Matrix testMatrix = new Matrix(new[]
            {
                new []{0,1,2,1},
                new []{2,0,7,-1},
                new []{6,5,0,2},
                new []{1,-1,4,0},
            });
            WeightedGraph graph = new WeightedGraph(testMatrix);
            var test = graph.GetFloydWarshallShortestPath();
            Console.OutputEncoding=Encoding.UTF8;
            Console.InputEncoding=Encoding.UTF8;
        }

        private static object[][] GetEmptyTable(Graph source)
        {
            object[][] bufferTable = Enumerable.Range(0, source.VertexesCount + 1)
                .Select(x => new object[source.VertexesCount + 1]).ToArray();
            for (int i = 1; i < source.VertexesCount + 1; i++)
            {
                bufferTable[0][i] = $"v{i}";
                bufferTable[i][0] = $"v{i}";
            }
            return bufferTable;
        }

        private static void PrintTable(object[][] source)
        {
            Console.WriteLine(string.Join("\n", source.Select(x=>string.Join("\t", x))));
        }
    }
}
