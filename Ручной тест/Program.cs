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
            //мой вариант
            Matrix testMatrix = new Matrix(new[]
            {
                new []{-1, 1, -1, -1, -1},
                new []{-1, -1, 3, -1, -1},
                new []{2 ,-1, -1, -1 ,4},
                new []{-1, -1, 1, -1, -1},
                new []{-1, -1, -1, 5, -1},
            });
            //Matrix testMatrix = new Matrix(new[]
            //{
            //    new []{-1, 3, -1},
            //    new []{1, 2, 4},
            //    new []{1 ,-1,-1},
            //});
            Console.WriteLine("Введите матрицу весов графа: ");
            Console.WriteLine(string.Join("\n", testMatrix.Values.Select(x => string.Join(" ", x))));
            Console.WriteLine();
            WeightedGraph graph = new WeightedGraph(testMatrix);
            Matrix result = graph.GetFloydWarshallShortestPath();
            //string testwat = graph.GetBellmanFordShortestPath();
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            
            Console.WriteLine("Матрица минимальных расстояний: ");
            object[][] raw = GetTable(result);
            Console.WriteLine(string.Join("\n", raw.Select(x => string.Join("\t", x))));
            Console.WriteLine();
        }

        private static object[][] GetTable(Matrix source)
        {
            object[][] bufferTable = Enumerable.Range(0, source.LengthY + 1)
                .Select(x => new object[source.LengthY + 1]).ToArray();
            for (int i = 1; i < source.LengthY + 1; i++)
            {
                bufferTable[0][i] = $"v{i}";
                bufferTable[i][0] = $"v{i}";
            }
            for(int i=0;i<source.LengthY;i++)
            for (int j = 0; j < source.LengthX; j++)
                bufferTable[i + 1][j + 1] = source[i, j];
            return bufferTable;
        }

        private static void PrintTable(object[][] source)
        {
            Console.WriteLine(string.Join("\n", source.Select(x => string.Join("\t", x))));
        }
    }
}
