using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using KMZILib;
using Vector = KMZILib.Vector;

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
            new[] {0.4, 0.5, 4, -8.5, 21.9},
            new[] {0.3, -1, 1, 5.2, -3.9},
            new[] {1, 0.2, 2.5, -1, 9.9}
        };
        static void Main(string[] args)
        {
            task2();
            return;
            Matrix A = new Matrix(new double[][] { new double[] { 1, 2.0 }, new double[] { 3, 4.0 }, new double[] { 5, 6.0 } });
            Matrix B = new Matrix(new double[][] { new double[] { 1, 3.0, 5 }, new double[] { 2, 4, 6.0 } });
            Console.WriteLine(A);
            Console.WriteLine("--------------");
            Console.WriteLine(B);
            Console.WriteLine("--------------");
            Console.WriteLine(A * B);
            //Matrix a = new Matrix(new[] { new[] { 1.0, 2 }, new[] { 3.0, 4 } });
            //Matrix b = new Matrix(new[] { new[] { 1.0, 2 }, new[] { 3.0, 4 } });
            //Matrix c = a + b;
            //Console.WriteLine(c);

            //KMZILib.Vector vector1 = new KMZILib.Vector(new[] { 1, 0, 1.0 });
            //KMZILib.Vector vector2 = new KMZILib.Vector(new[] { 1,1, 0, 0.0 });
            //Console.WriteLine(vector1-vector2);
        }

        public static void task2()
        {

            LinearEquations.LUMethod.Solve(TestArray);
        }

        public static void task1()
        {
            Vector GaussResult = LinearEquations.GaussMethod.Solve(TestArray,LinearEquations.GaussMethod.GEModification.LeadingOnWholeMatrix);
            Matrix GaussResultMatrix = new Matrix(new[] { GaussResult.Coordinates });
            Console.WriteLine($"Результат - {GaussResult}");
            Matrix A = new Matrix(TestArray.Select(row => row.Take(row.Length - 1).ToArray()).ToArray());
            Matrix B = new Matrix(TestArray.Select(row => row.Skip(row.Length - 1).ToArray()).ToArray());
            Matrix deltaB = B - A * GaussResultMatrix.TransposedCopy();
            double Delta = new Vector(deltaB).Length;
            double dB = Delta / new Vector(B).Length;
            Console.WriteLine($"Source B = {new Vector(B)}");
            Console.WriteLine($"newB = {new Vector(A * GaussResultMatrix.TransposedCopy())}\n\ndmB={dB}");
        }
    }
}
