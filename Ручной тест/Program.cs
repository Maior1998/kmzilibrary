using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KMZILib;
using KMZILib.Maths;

namespace Ручной_тест
{
    class Program
    {
        public static double[][] VarArray =
        {
            new[] {1, 2, 3, 9, 1.11},
            new[] {2, 1, 9, 4, 1.16},
            new[] {3, 9, 1, 4, 1.24},
            new[] {9, 1, 3, 4, 1.55}
        };

        public static double[][] TestArray =
        {
            new[] {2, 1, -1, 1, 2.7},
            new[] {0.4, 0.5, 4, -8.5,21.9},
            new[] {0.3, -1, 1, 5.2,-3.9},
            new[] {1, 0.2, 2.5, -1,9.9}
        };
        static void Main(string[] args)
        {
            //Console.WriteLine(string.Join("\n", LinearEquations.GaussianElimination(TestArray)));

            //Matrix a = new Matrix(new[] {new[] {1.0, 2}, new[] {3.0, 4}});
            //Matrix b = new Matrix(new[] { new[] { 1.0, 2 }, new[] { 3.0, 4 } });
            //Matrix c = a * b;

            KMZILib.Vector vector1 = new KMZILib.Vector(new[] { 1, 0, 1.0 });
            KMZILib.Vector vector2 = new KMZILib.Vector(new[] { 1,1, 0, 0.0 });
            Console.WriteLine(vector1-vector2);
        }
    }
}
