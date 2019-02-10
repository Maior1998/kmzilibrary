using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Представляет собой матрицу. Необязательно прямоугольную.
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Массив коэффициентов матрицы в виде вещественных чисел.
        /// </summary>
        public double[][] Values;

        /// <summary>
        /// Число строк матрицы.
        /// </summary>
        public int LengthY => Values.Length;

        /// <summary>
        /// Число столбцов матрицы. Вернёт -1, если нет строчек или если матрица не прямоугольная.
        /// </summary>
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

        /// <summary>
        /// Инициализирует новую матрицу с заданным массивом коэффициентов.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(double[][] Source)
        {
            Values = new double[Source.Length][];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = new double[Source[i].Length];
                Source[i].CopyTo(Values[i], 0);
            }
        }

        /// <summary>
        /// Инициализирует новую матрицу, которая является копией заданной матрицы.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(Matrix Source):this(Source.Values){}

        /// <summary>
        /// Инициализирует новую матрицу по заданному вектору значений.
        /// </summary>
        /// <param name="Source"></param>
        public Matrix(Vector Source):this(new[]{Source.Coordinates}){}

        /// <summary>
        /// Осуществляет умножение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix First, Matrix Second)
        {
            if (First.LengthX != Second.LengthY)
                throw new InvalidOperationException("Число столбцов первой матрицы и число строк второй не совпадают!");
            double[][] Result = new double[First.LengthY][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = new double[Second.LengthX];
            for (int ResultRow = 0; ResultRow < Result.Length; ResultRow++)
                for (int ResultColumn = 0; ResultColumn < Result.First().Length;ResultColumn++)
                {
                    double Sum = 0;
                    for (int i = 0; i < First.LengthX; i++)
                        Sum += First.Values[ResultRow][i] * Second.Values[i][ResultColumn];
                    Result[ResultRow][ResultColumn] = Sum;
                }
            return new Matrix(Result);
        }

        /// <summary>
        /// Осуществляет транспонирование текущей матрицы
        /// </summary>
        public Matrix Transpose()
        {
            double[][]NewMatrix=new double[LengthX][];
            for (int i = 0; i < NewMatrix.Length; i++)
            {
                NewMatrix[i]=new double[LengthY];
                for (int j = 0; j < NewMatrix[i].Length; j++)
                    NewMatrix[i][j] = Values[j][i];
            }
            Values = NewMatrix;
            return this;
        }

        /// <summary>
        /// Возвращает транспонированную копию текущей матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix TransposedCopy()
        {
            Matrix Copy = new Matrix(this);
            Copy.Transpose();
            return Copy;
        }

        /// <summary>
        /// Возвращает копию текущей матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            return new Matrix(this);
        }
        
        /// <summary>
        /// Переводит матрицу в формат вектора.
        /// </summary>
        /// <returns></returns>
        public Vector ToVector()
        {
            List<double>VectorArray = new List<double>();
            foreach (double[] row in Values) VectorArray.AddRange(row);
            return new Vector(VectorArray.ToArray());
        }

        /// <summary>
        /// Осуществляет сложение двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix First, Matrix Second)
        {
            if (First.LengthX != Second.LengthX ||
               First.LengthY != Second.LengthY)
                throw new InvalidOperationException("Размеры матриц должны быть одинаковыми.");
            double[][] Result = new double[First.LengthY][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = First.Values[i].Select((element, index) => element + Second.Values[i][index]).ToArray();
            return new Matrix(Result);
        }

        /// <summary>
        /// Осуществляет разность двух матриц и возвращает результат - новую матрицу.
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix First, Matrix Second)
        {
            if (First.LengthX != Second.LengthX ||
                First.LengthY != Second.LengthY)
                throw new InvalidOperationException("Размеры матриц должны быть одинаковыми.");
            double[][] Result = new double[First.LengthY][];
            for (int i = 0; i < Result.Length; i++)
                Result[i] = First.Values[i].Select((element, index) => element - Second.Values[i][index]).ToArray();
            return new Matrix(Result);
        }
        /// <summary>
        /// Возвращает строковое представление матрицы.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("\n", Values.Select(row => string.Join("\t", row)));
        }
    }
}
