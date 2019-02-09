using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    class Vector
    {
        public double[] Coordinates;

        public int CoordinatesLength => Coordinates.Length;

        public double Length => Math.Sqrt(Coordinates.Select(val => Math.Pow(val, 2)).Sum());

        public Vector(double[] coords)
        {
            Coordinates = new double[coords.Length];
            coords.CopyTo(Coordinates, 0);
        }

        public static Vector operator +(Vector First, Vector Second)
        {
            double[] Result=new double[Math.Max(First.CoordinatesLength,Second.CoordinatesLength)];

        }

        public override string ToString()
        {
            return $"({string.Join(", ", Coordinates)})";
        }
    }
}
