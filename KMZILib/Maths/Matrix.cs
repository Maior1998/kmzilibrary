using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib.Maths
{
    public class Matrix
    {
        public double[][] Values;

        public int LengthY => Values.Length;

        public int LengthX
        {
            get
            {
                if (LengthY == 0)
                    return -1;
                int Length = Values.First().Length;
                if (Values.Any(Doubles => Doubles.Length != Length))
                    return -1;
                return Length;
            }
        }

        public Matrix(double[][] Source)
        {
            Values = new double[Source.Length][];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new double[Source[i].Length];
                Source[i].CopyTo(Values[i], 0);
            }
        }

        public static Matrix operator *(Matrix First, Matrix Second)
        {
            if (First.LengthY != Second.LengthX)
                throw new InvalidOperationException("Число строк первой матрицы и число столбцов второй не совпадают!");
            double[][] Result = new double[First.LengthX][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = new double[Second.LengthY];
            for (int i = 0; i < Result.Length; i++)
                for (int j = 0; j < Result.First().Length; j++)
                    Result[i][j] = Second.Values.Select(row => row[j])
                        .Select((element, index) => element * First.Values[i][index]).Sum();
            return new Matrix(Result);
        }

    }
}
